# yUML-For-Visual-Studio
![Logo](http://i1.visualstudiogallery.msdn.s-msft.com/8dba2cec-c3c4-4782-950f-8396036635fc/image/file/109114/1/yuml_square.png)

- Very simply generate Class Diagram from code
- intuitively and rapidly,
- no compile, no reflection, just need a internet connect

 

If you are sadly facing some of old (or ugly) code, it can help to quickly show the relation for a class with other classes or interfaces;
If you are doing prototype design, and then you want a convenient talk about with your fellow, you can simply coding your prototype, and use it quickly make a beautiful diagram, then review it, share it, or print it;
If you are writing tech blog to express your idea,  and have be tired of paste code or hope a better intuitive expression, check out and try it;
 

The way use it, just click context menu item from a selected Class file node in Visual Studio Solution Explorer. The beautiful diagram will show in the inline-browser of Visual Studio. 

![snapshot10](http://i1.visualstudiogallery.msdn.s-msft.com/8dba2cec-c3c4-4782-950f-8396036635fc/image/file/109113/1/snapshot10.png)

![snapshot11](http://i1.visualstudiogallery.msdn.s-msft.com/8dba2cec-c3c4-4782-950f-8396036635fc/image/file/109111/1/snapshot11.png)

Use of http://yuml.me to generate class diagram. Thanks for Tobin Harris.

[Change Log]

- yUML Extension 0.94 (2013.08.06)

Add support to Visual Studio 2012, 2013

change icon and preview image

- yUML Extension 0.91

There was a known bug when you choose a file which have nested child files in Solution Explorer before v0.91.
eg. Resource.resx(Resource.Designer.cs) or Service1.cs(Service1.Designer.cs) or Form1.cs(Form1.Designer.cs)

If you encounter this problem, it have been fixed in v0.91.

- yUML Extension 0.8

Fixed many bugs, mainly for some Image View Tool impact browser to open .png files.
[Add]Save result to local temp folder for reuse and talk with others.
[Add]Show "yUML Expression".
Recomplie for VS2010 RTM.

- yUML Extension 0.7

Recomplie for VS2010 RC

- yUML Extension v0.6

Fix a little bug

- yUML Extension v0.5

Init publish
