# BookLibrary
MGM Solutions Assignment

Missing functionality:
1. Paging does not work with search at this point.
2. If someone tries to check out a book that was recently checked out by someone else, it fails with no error message (or possibly just lets the second person check it out).
3. Paging requires a button since I don't know how to trigger the post from the onchange in the SELECT at this point.

Set up:
1. From Tools -> NuGet Package Manager -> Package Manager Console in Visual Studio, run "Add-Migration InitialCreate -Context ApplicationDbContext".
2. From Package Manager Console, run "Add-Migration InitialCreate -Context BookContext" (note that there will be a bunch of warnings about my use of decimal).
3. From Package Manager Console, run "Create-Database -Context ApplicationDbContext" and then "Create-Database -Context BookContext".
4. At this point you should have aspnet-AuthData.mdf and aspnet-LibraryBookData.mdf database files in the App_Data folder.
5. Note that I also placed the book.csv file in the App_Data folder. Verify that the file is there.
6. Run the project and put in a route similar to https://localhost:7197/Home/PopulateDB which will run a method in the home controller to wipe out the db and populate it.
7. At this point you can create a user and login. 
