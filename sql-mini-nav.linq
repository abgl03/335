<Query Kind="Program">
  <Connection>
    <ID>cce011f0-21d9-4714-903b-ea955f848db7</ID>
    <Persist>true</Persist>
    <AttachFile>true</AttachFile>
    <AttachFileName>C:\usertmp\Northwind.mdf</AttachFileName>
    <Server>(localdb)\mssqllocaldb</Server>
  </Connection>
</Query>

void Main() {
    var nwd = this;
    
    var page = 2;
    var rowsPerPage = 3;
    var res = nwd.MyOrders
        .OrderBy(o => o.MyCustomers.Age)
        .ThenBy(o => o.OrderID)
        .Skip((page - 1) * rowsPerPage)
        .Take(rowsPerPage)
        .Select(o => new MyModel {
            OrderID = o.OrderID,
            Description = o.Description,
            Price = o.Price,
            Items = o.Items,
            CustomerID = o.MyCustomers.CustomerID,  // s
            Name = o.MyCustomers.Name,  // s
            Age = o.MyCustomers.Age,  // s
        });
        
   res.Dump("res");
}

public class MyModel {
    public string OrderID { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Items { get; set; }
    public string CustomerID { get; set; }
    public string Name { get; set; }
    public int? Age { get; set; }
}