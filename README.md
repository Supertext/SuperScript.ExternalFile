_**IMPORTANT NOTE:**_ This project is currently in beta and the documentation is currently incomplete. Please bear with us while the documentation is being written.

####SuperScript offers a means of declaring assets in one part of a .NET web solution and have them emitted somewhere else.


When developing web solutions, assets such as JavaScript declarations or HTML templates are frequently written in a location that differs from their desired output location.

For example, all JavaScript declarations should ideally be emitted together just before the HTML document is closed. And if caching is preferred then these declarations should be in an external file with caching headers set.

This is the functionality offered by SuperScript.



##Relocate Assets into a Seperate File

The most obvious use case for this is JavaScript. Why write lots of oft-used JavaScript in the _.html_ file when SuperScript
can move this into a seperate file? Not only does this make the JavaScript elegibile for caching but it also allows for 
more secure webpages through use of HTML5's [Content Security Policy](http://en.wikipedia.org/wiki/Content_Security_Policy).

To implement this a developer can make JavaScript declarations as with other SuperScript assemblies (for example, see
[`SuperScript.JavaScript`](https://github.com/Supertext/SuperScript.JavaScript)). Where `SuperScript.ExternalFile` helps,
in general, is in providing a storage means for the files and an implementation of `SuperScript.Modifiers.ModifierBase` which
is the object that does the relocation.

`SuperScript.ExternalFile` also includes handlers for handling the HTTP request for the seperate file.

Furthermore, `SuperScript.ExternalFile` also comes with a web interface for monitoring and managing the store where the 
seperate files live.

_**IMPORTANT NOTE:**_ A future version of `SuperScript.ExternalFile` will have the intelligence to delete the files once 
their lifetime has run. Until then, the files need to be manually deleted.

###Storage Options

`SuperScript.ExternalFile` comes with three storage options:

* Database storage

  This allows the contents of the seperate file to be stored in a database. See `SuperScript.ExternalFile.MySql` and 
  `SuperScript.ExternalFile.SqlServer`.
  This requires implementations of `SuperScript.ExternalFile.Storage.IDbStore` and `IDbStoreProvider`, which allows for 
  third party database storage providers to be built.

* File system storage

  This implementation stores 'physical files' on the host server's file system.

* Isolated storage

  This implementation uses [isolated storage](http://www.techopedia.com/definition/24291/isolated-storage-net) 
  for added security.


##What's in this project?

Below is a list of some of the more important classes in the `SuperScript.ExternalFile` project.

* `SuperScript.ExternalFile.Modifiers.Writers.ExternalScriptWriter`

  An implementation of `SuperScript.Modifiers.ModifierBase`. An emitter (_i.e._, an implementation of `SuperScript.Emitters.IEmitter`)
  should be declared, citing this class as the implementation of `HtmlWriter`, _i.e._,
  
  ```C#
  IEmitter.HtmlWriter = new SuperScript.ExternalFile.Modifiers.Writers.ExternalScriptWriter();
  ```

* Storage providers
  * `SuperScript.ExternalFile.Storage.DbStore`
  * `SuperScript.ExternalFile.Storage.FileStore`
  * `SuperScript.ExternalFile.Storage.IsoStore`
  
  See the section above ([Storage Options](#storage-options))
  for a summary of these.
  
* `SuperScript.ExternalFile.Storage.Storable`

  An object of this type represents a 'storable item', that is, the contents which should be stored, the lifetime, 
  content type, _etc._

* `SuperScript.ExternalFile.Storage.IStore`

  Implementations of this interface may be configured as the underlying store for the contents of seperate files.

* File handlers

  `SuperScript.ExternalFile` provides the means for handling the HTTP requests for these seperate files, as well as their wiring.
  
  Handlers are also provided for the webpage interface which can be used for managing the underlying storage.

* Configuration classes

  SuperScript can be configured inside a _.config_ file. This allows emitters and declarations to be modified without 
  rebuilding the assembly. `SuperScript.ExternalFile` contains the classes needed for configuring the storage in a 
  _.config_ file.
  
  [SuperScript does not _need_ to be configured in a _.config_ class.]


##Dependencies
There are a variety of SuperScript projects, some being dependent upon others.

* [`SuperScript.Common`](https://github.com/Supertext/SuperScript.Common)

  This library contains the core classes which facilitate all other SuperScript modules but which won't produce any meaningful output on its own.
  

`SuperScript.ExternalFile` has been made available under the [MIT License](https://github.com/Supertext/SuperScript.ExternalFile/blob/master/LICENSE).
