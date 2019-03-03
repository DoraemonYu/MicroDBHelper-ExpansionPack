![icon](https://github.com/DoraemonYu/MicroDBHelper-ExpansionPack/blob/gh-pages/icons/EntityConversion.png?raw=true)  
# ExpansionPack :: EntityConversion  
This expansion pack is focus on convert datas between datatable object and entity model list. 

*The root section of documents of **all optional expansion packs**, please visit [here](/MicroDBHelper-ExpansionPack/).*


## Generated for Framework Version:
* .NET Framework 2.0
* .NET Framework 3.5
* .NET Framework 3.5 Client Profile
* .NET Framework 4.0
* .NET Framework 4.0 Client Profile
* .NET Framework 4.5
* .NET Framework 4.6 
* .NET Standard 2.0


## Dependencies 
None.


## Usage

### Define your model class 
Firstly, define the data model class. Set the target **propertys** to **Public**, that is ready to automatically map with the data column. 
For example, 
``` 
class Model
{
    public int ID { get; set; }
    public string Name { get; set; }
}
``` 

### Call conversion method 
There are two alternatives: 
* **Static method**.  

  * `EntityConvert.ConvertToList<Model>(DataTableObject);`  Pass the model's type to **Generic Type Parameter** and DataTable object to **Method Parameter**. It will return a collection with type of `IList<Model>`.
  * `EntityConvert.ConvertToDatatable(EntityList);` Pass the `IEnumerable<Model>`*(this also Include `IList<Model>` and `Model[]`)* to **Method Parameter**. It will return a datatable object.
 
* **Extended method**.   

  * `DataTableObject.ToList<Model>();`  Using the namespaces of **System.Data** (the namespaces same as DataTable) in your code file, then you can use this way.  It will return a collection with type of `IList<Model>`.
  * `IEnumerable<Model>.ToDatatable();` Using the namespaces of **System.Data** (the namespaces same as DataTable) in your code file, then you can use this way.  It will return a datatable object.
  
  



### Mapping control
Control the behaviors of the mapping process. 

#### Matched name control 
By default, library will Compare **colnum name of DataTable Object** to **property name of Model Instance**. If both match, then will set the value. 

However, in some scenes, maybe you hope to use another name to be matched, then you can use the **MicroDBHelpers.ExpansionPack.ColumnAttribute** associated to the target property, for example:  
```
class Model
{
    [Column("Identity")]
    public int ID { get; set; }
}
```

#### Case sensitive control
By default, it is **case sensitive**. If you hope the library ignore case when compare names, there are two alternatives: 
* use the **MicroDBHelpers.ExpansionPack.ColumnAttribute** associated to each expected propertys, and set the **CaseSensitiveToMatchedName** attribute to **false** ( by default it is true ); 
* call `MicroDBHelpers.ExpansionPack.EntityConversionDefaultSettings.CaseSensitiveToColumnName = false;` ( by default it is true ) , this will effect all propertys which is not associated by ColumnAttribute; 

Please note again, if you set the `EntityConversionDefaultSettings.CaseSensitiveToColumnName` and use `ColumnAttribute` **both**, then the finall rule of Case sensitive to **that one property** would in `ColumnAttribute` prevail.  



#### Ignore specified column
Sometimes,you may hope to ignore some columns, either convert to entity list, or convert to datatable. 

To do this, just need to use the MicroDBHelpers.ExpansionPack.IgnoreAttribute associated to the expected propertys.






## Download compiled binary file
If you needn't to got the code and bulid by yourself for the moment, I also offer the newest compiled file in the BUILD directiory for your convenience. 

[Build Directory](https://github.com/DoraemonYu/MicroDBHelper-ExpansionPack/tree/master/Build)


## NuGet 
`Install-Package MicroDBHelper-ExpansionPack-EntityConversion`  [link](https://www.nuget.org/packages/MicroDBHelper-ExpansionPack-EntityConversion/)

<br><br><br>
o(∩_∩)o *The root section of documents of **all optional expansion pack**, please visit [here](/MicroDBHelper-ExpansionPack/).*
