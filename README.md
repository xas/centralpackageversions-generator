# CentralPackageVersions Generator

Dirty code which parse a root folder for all csproj files available :

* update the `path` variable to the root of your solution
* find the `<PackageReference>` line
* save it
* create a new _Packages.props_ file at the root folder
* create an _ItemGroup_ element
* add all the `<PackageReference>` found
* replace `Include` with `Update` attribute
* define `Version` as an attribute (instead of a child element)
* save
* remove the `Version` attribute in the `<PackageReference>` from all the csproj files

You still need to create the _Directory.Build.targets_ file with this content :

```xml
<Project>
  <Sdk Name="Microsoft.Build.CentralPackageVersions" Version="2.1.3" />
</Project>
```

## TODO

I was not able to set the `Update` attribute **before** the `Version` attribute.  
I didn't look more than that, I don't know if it's possible or not.
