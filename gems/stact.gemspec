version = File.read(File.expand_path("../VERSION",__FILE__)).strip

Gem::Specification.new do |spec|
  spec.platform    = Gem::Platform::RUBY
  spec.name        = 'Stact'
  spec.version     = version
  spec.files = Dir['lib/**/*']
  
  spec.summary     = 'Stact - The library for the larger than average developer'
  spec.description = 'Stact is a library of classes for building .NET applications'
  
  spec.authors           = ['Chris Patterson','Dru Sellers']
  spec.email             = 'Stact@Stact-project.net'
  spec.homepage          = 'http://Stact-project.net'
  spec.rubyforge_project = 'Stact'
end