![icon](https://github.com/DoraemonYu/MicroDBHelper-ExpansionPack/blob/gh-pages/icons/TransactionWrap.png?raw=true)  
# ExpansionPack :: TransactionWrap 
 
This expansion pack is focus on "Assembly Decoupling". Allow assembly which is hierarchical design, conveniently to create Transaction Object, without Reference the *"MicroDBHelper.dll"*. 


*The root section of documents of **all optional expansion packs**, please visit [here](/MicroDBHelper-ExpansionPack/).*


## Environmental Requirement
* .Net framework 4.5 and +
 
  
  
## Background 
MicroDBHelper offer friendly interfaces to use DBHelper, and offer Transaction Support as well. **All of them** are packaged in the single *"MicroDBHelper.dll"* . After reference it, you can use it anywhere.   

However, when you use it in a hierarchical design project, then you will face some Closure problem.  
  
   
   
For excample ( an 3-tier architecture project):  
![snapshot](images/TransactionWrap/REFERENCE_BEFORE.png)  
We keep the database logics only in the *DataAccess Layer*, but may allow Transaction begin in *Business Layer*, even *Application Layer*.   

As you see, due to the " All of them are packaged in the single dll", you SHOULD reference it again in *Business Layer* and up layers, just in order to use *MicroDBTransaction*.  

Maybe we can solve this problem by some design patterns or development agreements, but it's not very rigorous for the **Closures Principle**, isn't it?  

Therefore, this library offer a solution with assembly-level to solve it. Let us look this:  
![snapshot](images/TransactionWrap/REFERENCE_AFTER.png)   
Now, we use an wrap from this expansion pack to instead of *MicroDBTransaction* from *"MicroDBHelper.dll"*. It just an wrap, no more logics in it and just make the assemblys to be Decoupling.  

*Business Layer* and up layers use *TransactionWrap* type ; Public interfaces of *DataAccess Layer* also use *TransactionWrap* type, and use *MicroDBTransaction* inner and finally invoke with *MicroDBHelper*, library will automatically identify *TransactionWrap* as *MicroDBTransaction* .


## Usage

### Implicit conversion 
:) **Just feel free** to use *TransactionWrap*.   
Because of library will automatically identify *TransactionWrap* as *MicroDBTransaction*.   
  
So firstly, all the usages with properties and methods in *TransactionWrap* is same as them in *MicroDBTransaction* ( [link](https://doraemonyu.github.io/MicroDBHelper/#transaction) ) ; Secondly, you could just pass the *TransactionWrap* instance to those methods that is define *MicroDBTransaction* type parameter without any manual conversion.

### Different
The **tiny** difference in code is that when begin Transaction, use *TransactionWrap.UseTransaction* instead of *MicroDBHelper.UseTransaction*. ( When you modify codes, you can do this with the bulk text replacement tool.  )

 
  
## Notes & Recommend
This expansion pack was an wrap and finally call logics in *"MicroDBHelper.dll"* as well.   
So just one thing to do is, make sure that assemblies of current AppDomain MUST at least one reference the *"MicroDBHelper.dll"*.   

Like the example above, *DataAccess Layer*  is part of the AppDomain and reference *"MicroDBHelper.dll"* , then other layers who use the wrap whitout reference *"MicroDBHelper.dll"* wolud still work properly.




## Download compiled binary file
If you needn't to got the code and bulid by yourself for the moment, I also offer the newest compiled file in the BUILD directiory for your convenience. 

[Build Directory](https://github.com/DoraemonYu/MicroDBHelper-ExpansionPack/tree/master/Build)


## NuGet 
`Install-Package MicroDBHelper-ExpansionPack-TransactionWrap`  [link](https://www.nuget.org/packages/MicroDBHelper-ExpansionPack-TransactionWrap/)

<br><br><br>
o(∩_∩)o *The root section of documents of **all optional expansion pack**, please visit [here](/MicroDBHelper-ExpansionPack/).*
