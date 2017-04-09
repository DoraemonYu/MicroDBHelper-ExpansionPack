ExpansionPack :: PagingQuery 
================================================ 
 
This expansion pack is focus on paging query.  

*The root section of documents of **all optional expansion pack**, please visit [here](/MicroDBHelper-ExpansionPack/).*


## Environmental Requirement
* .Net framework 4.5 and +


## Usage

### Result container 
Firstly, let us see the result models,  `PagingResult` and `PagingResult<T>`.  

The former include **DataTable object** result, the latter include **entity list of target type** result.  

|   | PagingResult | PagingResult&lt;T&gt; |
| ------| ------ | ------ |
| .Datas | DataTable | IList&lt;T&gt; |
| .CurrentPageIndex | Current Index | Current Index |
| .PageSize | Size of per Page | Size of per Page |
| .TotalItemsCount | Count of all items | Count of all items |
| .TotalPages | Count of all pages | Count of all pages |




### Call query method 
#### Paging datas by Database
There are two alternatives in **MicroDBHelpers.ExpansionPack.PagingQuerier** static class : 
* `PagingAsDatatable` and `PagingAsDatatableAsync`. This will return `PagingResult` (DataTabel result in **Datas** property);
 
* `PagingAsEntity` and `PagingAsEntityAsync`. This will return `PagingResult<T>` (entity list of target type in **Datas** property), **note** that you need to reference [EntityConversion](/MicroDBHelper-ExpansionPack/EntityConversion/) when you choose this alternative ;
 
 
There are some key parameters in these query method:
* **pageIndex** and **pageSize**. Indicate how you want to paginate. 

  *BTW, it's useful that you can pass a big number to **pageSize** (such as int.MaxValue) in order to get all datas in some scenes.*

* **fixedSql**. If your SQL expression include somethings that front before SELECT (such as CTE, Variable definitions, etc. ) then you can put them in this parameter； if not include, just pass String.Empty;
 
* **selectSql**. It is the core part your SELECT expression. The library requires it to contain **SELECT** and **FROM*** keywords, **ORDER BY** is Optional.

More about his **fixedSql** and  **selectSql** : 

![snapshot](images/PagingQuery/part_sqls.PNG)

#### Directly paging entities In Memory
This is *Just a helper function* for developers who hope to "Paging Datas in Memory" and use the "PagingResult Model". 

```
//Method definition：
PagingResult<T> PagingByList<T>(IEnumerable<T> datas, int pageIndex, int pageSize);
```





## Download compiled binary file
If you needn't to got the code and bulid by yourself for the moment, I also offer the newest compiled file in the BUILD directiory for your convenience. 

[Build Directory](https://github.com/DoraemonYu/MicroDBHelper-ExpansionPack/tree/master/Build)


## NuGet 
!!This section will be supplemented in the near future.!!  

`Install-Package MicroDBHelper-ExpansionPack-PagingQuery`  [link](https://www.nuget.org/packages/MicroDBHelper-ExpansionPack-PagingQuery/)

<br><br><br>
o(∩_∩)o *The root section of documents of **all optional expansion pack**, please visit [here](/MicroDBHelper-ExpansionPack/).*
