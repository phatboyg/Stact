version = File.read(File.expand_path("../VERSION",__FILE__)).strip

Gem::Specification.new do |spec|
  spec.platform    = Gem::Platform::RUBY
  spec.name        = 'magnum'
  spec.version     = version
  spec.files = Dir['lib/**/*']
  
  spec.summary     = 'Magnum - The library for the larger than average developer'
  spec.description = 'Magnum is a library of classes for building .NET applications'
  
  spec.authors           = ['Chris Patterson','Dru Sellers']
  spec.email             = 'magnum@magnum-project.net'
  spec.homepage          = 'http://magnum-project.net'
  spec.rubyforge_project = 'magnum'
end