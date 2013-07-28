This project uses the LINQ 2 XSD tools to generate strongly typed classes on top of LINQ to XML for the XSD schema file included.
Since the included schema does not specify a namespace, it is best to keep these generated classes in a seperate assembly 
so that we can use extern alias to avoid C# namespace conflicts between generated classes and other classes in the .NET framework/current solution.