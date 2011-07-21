COPYRIGHT = "Copyright 2010-2011 Chris Patterson - All rights reserved."

require File.dirname(__FILE__) + "/build_support/BuildUtils.rb"

include FileTest
require 'albacore'
require File.dirname(__FILE__) + "/build_support/ilmergeconfig.rb"
require File.dirname(__FILE__) + "/build_support/ilmerge.rb"

BUILD_NUMBER_BASE = '1.0.0'
PRODUCT = 'Stact'
CLR_TOOLS_VERSION = 'v4.0.30319'

BUILD_CONFIG = ENV['BUILD_CONFIG'] || "Release"
BUILD_CONFIG_KEY = ENV['BUILD_CONFIG_KEY'] || 'NET40'
BUILD_PLATFORM = ''
TARGET_FRAMEWORK_VERSION = (BUILD_CONFIG_KEY == "NET40" ? "v4.0" : "v3.5")
MSB_USE = (BUILD_CONFIG_KEY == "NET40" ? :net4 : :net35)
OUTPUT_PATH = (BUILD_CONFIG_KEY == "NET40" ? 'net-4.0' : 'net-3.5')

props = {
  :src => File.expand_path("src"),
  :build_support => File.expand_path("build_support"),
  :stage => File.expand_path("build_output"),
  :output => File.join( File.expand_path("build_output"), OUTPUT_PATH ),
  :artifacts => File.expand_path("build_artifacts"),
  :projects => ["Stact", "Stact.ServerFramework"]
}

desc "Cleans, compiles, il-merges, unit tests, prepares examples, packages zip and runs MoMA"
task :all => [:default, :package, :moma]

desc "**Default**, compiles and runs tests"
task :default => [:clean, :compile, :ilmerge, :tests]

desc "**DOOES NOT CLEAR OUTPUT FOLDER**, compiles and runs tests"
task :unclean => [:compile, :ilmerge, :tests]

desc "Update the common version information for the build. You can call this task without building."
assemblyinfo :global_version do |asm|
  asm_version = BUILD_NUMBER_BASE + ".0"
  commit_data = get_commit_hash_and_date
  commit = commit_data[0]
  commit_date = commit_data[1]
  build_number = "#{BUILD_NUMBER_BASE}.#{Date.today.strftime('%y%j')}"
  tc_build_number = ENV["BUILD_NUMBER"]
  build_number = "#{BUILD_NUMBER_BASE}.#{tc_build_number}" unless tc_build_number.nil?

  puts "Setting assembly file version to #{build_number}"

  # Assembly file config
  asm.product_name = PRODUCT
  asm.description = "Git commit hash: #{commit} - #{commit_date} - Stact - An actor library and framework. http://github.com/phatboyg/Stact"
  asm.version = asm_version
  asm.file_version = build_number
  asm.custom_attributes :AssemblyInformationalVersion => "#{asm_version}",
	:ComVisibleAttribute => false,
	:CLSCompliantAttribute => false
  asm.copyright = COPYRIGHT
  asm.output_file = 'src/SolutionVersion.cs'
  asm.namespaces "System", "System.Reflection", "System.Runtime.InteropServices", "System.Security"
end

desc "Prepares the working directory for a new build"
task :clean do
	FileUtils.rm_rf props[:artifacts]
	FileUtils.rm_rf props[:stage]
	# work around latency issue where folder still exists for a short while after it is removed
	waitfor { !exists?(props[:stage]) }
	waitfor { !exists?(props[:artifacts]) }

	Dir.mkdir props[:stage]
	Dir.mkdir props[:artifacts]
end

desc "Cleans, versions, compiles the application and generates build_output/."
task :compile => [:global_version, :build] do
	puts 'Copying unmerged dependencies to output folder'

	copyOutputFiles File.join(props[:src], "Stact/bin/#{BUILD_CONFIG}"), "Stact.{dll,pdb,xml}", props[:output]
	copyOutputFiles File.join(props[:src], "Stact/bin/#{BUILD_CONFIG}"), "Magnum.{dll,pdb,xml}", props[:output]
	copyOutputFiles File.join(props[:src], "Stact/bin/#{BUILD_CONFIG}"), "log4net.{dll,pdb,xml}", props[:output]

end

task :ilmerge => [:ilmerge_server] do
end


ilmerge :ilmerge_server do |ilm|
	out = File.join(props[:output], 'Stact.ServerFramework.dll')
	ilm.output = out
	ilm.internalize = File.join(props[:build_support], 'internalize.txt')
	ilm.working_directory = File.join(props[:src], "Stact.ServerFramework/bin/#{BUILD_CONFIG}")
	ilm.target = :library
        ilm.use MSB_USE
	ilm.log = File.join( props[:src], "Stact.ServerFramework","bin","#{BUILD_CONFIG}", 'ilmerge.log' )
	ilm.allow_dupes = true
	ilm.union = false
	ilm.references = [ 'Stact.ServerFramework.dll', 'Newtonsoft.Json.dll']
end


ilmerge :ilmerge_stact do |ilm|
	out = File.join(props[:output], 'Stact.dll')
	ilm.output = out
	ilm.internalize = File.join(props[:build_support], 'internalize.txt')
	ilm.working_directory = File.join(props[:src], "Stact/bin/#{BUILD_CONFIG}")
	ilm.target = :library
        ilm.use MSB_USE
	ilm.log = File.join( props[:src], "Stact","bin","#{BUILD_CONFIG}", 'ilmerge.log' )
	ilm.allow_dupes = false
	ilm.union = true
	ilm.references = [ 'Stact.dll', 'Magnum.dll']
end


desc "Only compiles the application."
msbuild :build do |msb|
	msb.properties :Configuration => BUILD_CONFIG,
	    :BuildConfigKey => BUILD_CONFIG_KEY,
	    :TargetFrameworkVersion => TARGET_FRAMEWORK_VERSION,
	    :Platform => 'Any CPU'
	msb.properties[:TargetFrameworkVersion] = TARGET_FRAMEWORK_VERSION unless BUILD_CONFIG_KEY == 'NET35'
	msb.use :net4 #MSB_USE
	msb.targets :Clean, :Build
	msb.solution = 'src/Stact.sln'
end

def copyOutputFiles(fromDir, filePattern, outDir)
	FileUtils.mkdir_p outDir unless exists?(outDir)
	Dir.glob(File.join(fromDir, filePattern)){|file|
		copy(file, outDir) if File.file?(file)
	}
end

task :tests => [:unit_tests]

desc "Runs unit tests (integration tests?, acceptance-tests?) etc."
task :unit_tests => [:compile] do
	Dir.mkdir props[:artifacts] unless exists?(props[:artifacts])

	runner = NUnitRunner.new(File.join('lib', 'nunit', 'net-2.0',  "nunit-console#{(BUILD_PLATFORM.empty? ? '' : "-#{BUILD_PLATFORM}")}.exe"),
		'tests',
		TARGET_FRAMEWORK_VERSION,
		['/nothread', '/nologo', '/labels', "\"/xml=#{File.join(props[:artifacts], 'nunit-test-results.xml')}\""])

	runner.run ['Stact.Specs'].map{ |assem| "#{assem}.dll" }
end

desc "Target used for the CI server. It both builds, tests and packages."
task :ci => [:default, :package, :moma]

task :package => [:zip_output, :nuget]

desc "ZIPs up the build results and runs the MoMA analyzer."
zip :zip_output do |zip|
	zip.directories_to_zip = [props[:stage]]
	zip.output_file = "Stact-#{BUILD_NUMBER_BASE}.zip"
	zip.output_path = [props[:artifacts]]
end

desc "Runs the MoMA mono analyzer on the project files. Start the executable manually without --nogui to update the profiles once in a while though, or you'll always get the same report from the analyzer."
task :moma => [:compile] do
	puts "Analyzing project fitness for mono:"
	dlls = project_outputs(props).join(' ')
	sh "lib/MoMA/MoMA.exe --nogui --out #{File.join(props[:artifacts], 'MoMA-report.html')} #{dlls}"
end

# TODO: create tasks for installing and running samples!

desc "Builds the nuget package"
task :nuget do
	sh "lib/nuget pack stact.nuspec /OutputDirectory build_artifacts"
end

def project_outputs(props)
	props[:projects].map{ |p| "src/#{p}/bin/#{BUILD_CONFIG}/#{p}.dll" }.
		concat( props[:projects].map{ |p| "src/#{p}/bin/#{BUILD_CONFIG}/#{p}.exe" } ).
		find_all{ |path| exists?(path) }
end

def get_commit_hash_and_date
	begin
		commit = `git log -1 --pretty=format:%H`
		git_date = `git log -1 --date=iso --pretty=format:%ad`
		commit_date = DateTime.parse( git_date ).strftime("%Y-%m-%d %H%M%S")
	rescue
		commit = "git unavailable"
	end

	[commit, commit_date]
end

def waitfor(&block)
	checks = 0

	until block.call || checks >10
		sleep 0.5
		checks += 1
	end

	raise 'Waitfor timeout expired. Make sure that you aren\'t running something from the build output folders, or that you have browsed to it through Explorer.' if checks > 10
end
