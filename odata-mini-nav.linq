<Query Kind="Program">
  <Connection>
    <ID>238752c3-10a8-45d9-9a4a-cc175dd18dbe</ID>
    <Persist>true</Persist>
    <Driver>AstoriaAuto</Driver>
    <Server>http://localhost:8181/WcfDataService1.svc/</Server>
  </Connection>
</Query>

void Main() {
    var nwd = this;
    
    var page = 2;
    var rowsPerPage = 3;
    var res = nwd.MyOrders
        .OrderBy(o => o.Price)
        .ThenBy(o => o.OrderID)
        .Skip((page - 1) * rowsPerPage)
        .Take(rowsPerPage)
        .Select(o => new MyModel {
            OrderID = o.OrderID,
            Description = o.Description,
            Price = o.Price,
            Items = o.Items,
            CustomerID = o.MyCustomer.CustomerID,
            Name = o.MyCustomer.Name,
            Age = o.MyCustomer.Age,
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
