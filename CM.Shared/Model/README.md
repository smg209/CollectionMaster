# Model

`GeneratedClasses_CM.cs` belongs in this folder. It is produced by the DBQ schema reader
(PagSchema) from the CollectionMaster database and must never be written or edited by hand:
change the schema, regenerate, and replace the file.

The generated classes are partial. Business logic goes in separate partial class files beside
the generated one. All of them use the flat `CM.Shared` namespace.
