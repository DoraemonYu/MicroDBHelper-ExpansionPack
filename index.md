![icon](https://github.com/DoraemonYu/MicroDBHelper-ExpansionPack/blob/gh-pages/icons/packs.png?raw=true)  
# MicroDBHelper-ExpansionPack

These are Optional Expansion Packs for [MicroDBHelper](https://doraemonyu.github.io/MicroDBHelper/) , which is a friendly interface library to use SQLHelper. 
 

## Background
MicroDBHelper is target to make a **lightweight and friendly** library, so the core Features are only Focus on Pure SqlHelper, and without **some related features** (such as Entity Conversion, Paging Query , etc.) which is useful in the development.   

In fact, for the original intention of the design, developers, who use MicroDBHelper library, can use their demand logic to simply write , or use some other libraries which is good at specific areas , to achieve those related features.  
However, some developers hope I could provide some related features for MicroDBHelper. Then they can relatively smooth to learn and use MicroDBHelper library in the early. And then consider whether to replace them, the related features, in the later.



## Optional Expansion List
Click the link of item below to see its document: 

* [EntityConversion](/MicroDBHelper-ExpansionPack/EntityConversion) It is focus on convert datas between datatable object and entity model list;

* [PagingQuery](/MicroDBHelper-ExpansionPack/PagingQuery) It is focus on paging query;

* [TransactionWrap](/MicroDBHelper-ExpansionPack/TransactionWrap) Assembly decoupling. Allow assembly which is hierarchical design, conveniently to create Transaction Object, without Reference the [MicroDBHelper] DLL.


## LICENSE
MIT 

Although LICENSE of MicroDBHelper is LGPL, the MicroDBHelper-ExpansionPack is MIT :) 

Most of the logics in these Optional Expansion Packs is generic, the core logics can be used for others projects easily with few modify and it's intention was not just for MicroDBHelper.  
So you can get these source codes to your projects for personal or businesses, and join your valuable idea and modify, share them to everyone if you like.
