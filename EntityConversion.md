ExpansionPack :: EntityConversion 
================================================ 
 
This expansion pack is focus on convert DataTable object to target entity model.  

*The root section of documents of **all optional expansion pack**, please visit [here](https://doraemonyu.github.io/MicroDBHelper-ExpansionPack/).*


## Environmental Requirement
* .Net framework 4.0 and +


## Usage

### Define your model class 
Firstly, define the data model class. Set the target **propertys** to **Public**, which is ready to automatically map with the data column. 
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
* **Static method**. `EntityConvert.ConvertToList<Model>(DataTableObject);`  Pass the model's type to **Generic Type Parameter** and DataTable object to **Method Parameter**. Then it will return a collection with type of `IList<Model>`.
 
* **Extended method**. `DataTableObject.ToList<Model>();` using the namespaces of **System.Data** (the namespaces same as DataTable) in your code file, then you can use this way.  Then it will return a collection with type of `IList<Model>`.



### Mapping contrl
Control the behaviors of the mapping process. 

#### Matched name control 
By default, library will Compare **colnum name of DataTable Object** to **property name of Model Instance**. If both match, then will set the value. 

However, in some scenes, maybe you hope to use another name to be matched, then you can use the **MicroDBHelpers.ExpansionPack.ColumnAttribute** associated to the property, for example:  
```
class Model
{
    [Column("Identity")]
    public int ID { get; set; }
}
```

#### Case sensitive control
By default, it is **case sensitive**. If you hope the library ignore case when compare names, there are two alternatives: 
* use the **MicroDBHelpers.ExpansionPack.ColumnAttribute** associated to each expected propertys, and set the **CaseSensitiveToMatchedName** to **false**; 
* call `MicroDBHelpers.ExpansionPack.EntityConversionDefaultSettings.CaseSensitiveToColumnName = false;` , this will effect all propertys which is not associated by ColumnAttribute; 

Please note again, if you set the `EntityConversionDefaultSettings.CaseSensitiveToColumnName` and use `ColumnAttribute` **both**, then the finall rule of Case sensitive to **that one property** would in `ColumnAttribute` prevail. 





## Download compiled binary file
If you needn't to got the code and bulid by yourself for the moment, I also offer the newest compiled file in the BUILD directiory for your convenience. 

[Build Directory](https://github.com/DoraemonYu/MicroDBHelper-ExpansionPack/tree/master/Build)


## NuGet 
!!This section will be supplemented in the near future.!!  

`Install-Package MicroDBHelper-ExpansionPack-EntityConversion`  [link](https://www.nuget.org/packages/MicroDBHelper-ExpansionPack-EntityConversion/)

<br><br><br>
o(∩_∩)o *The root section of documents of **all optional expansion pack**, please visit [here](https://doraemonyu.github.io/MicroDBHelper-ExpansionPack/).*
