<Query Kind="Program" />

void Main() {
    var _MyOrders =
        XElement.Load(@"C:\usertmp\XMyOrders.xml")
        .Elements("MyOrder")
        .Select(x => new MyOrder {
            OrderID = (int)x.Element("OrderID"),
            CustomerID = (string)x.Element("CustomerID"),
            Description = (string)x.Element("Description"),
            Price = (decimal)x.Element("Price"),
            Items = (int)x.Element("Items"),
        }).ToArray();
    
    var _MyCustomers =
        XElement.Load(@"C:\usertmp\XMyCustomers.xml")
        .Elements("MyCustomer")
        .Select(x => new MyCustomer {
            CustomerID = (string)x.Element("CustomerID"),
            Name = (string)x.Element("Name"),
            Age = string.IsNullOrEmpty((string)x.Element("Age")) ? null : (int?)x.Element("Age"),
        }).ToArray();

    //_MyOrders.Dump("_MyOrders");
    //_MyCustomers.Dump("_MyCustomers");

    var _os = _MyOrders.ToLookup(o => o.CustomerID);
    var _cs = _MyCustomers.ToDictionary(c => c.CustomerID);
    //_os.Dump("_os");
    //_cs.Dump("_cs");
    foreach (var o in _MyOrders) o.Customer = _cs[o.CustomerID];
    foreach (var c in _MyCustomers) c.Orders = _os[c.CustomerID];
    
    //_MyOrders.Dump("_MyOrders");
    //_MyCustomers.Dump("_MyCustomers");

    var page = 2;
    var rowsPerPage = 3;
    var res = _MyOrders
        .OrderBy(o => o.OrderID)
        .Skip((page - 1) * rowsPerPage)
        .Take(rowsPerPage)
        .Join(_MyCustomers, o => o.CustomerID, c => c.CustomerID, 
            (o,c) => new MyJoin {
                OrderID = o.OrderID, 
                CustomerID = o.CustomerID, 
                Name = c.Name, 
                Age = c.Age,
                Description = o.Description,
                Price = o.Price,
                Items = o.Items,
            })
        ;
        
   res.Dump("res");
}

public class MyJoin {
    public int OrderID { get; set; }
    public string CustomerID { get; set; }
    public string Name { get; set; }
    public int? Age { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Items { get; set; }
}

public class MyOrder {
    public int OrderID { get; set; }
    public string CustomerID { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Items { get; set; }
    public MyCustomer Customer { get; set; }
}

public class MyCustomer {
    public string CustomerID { get; set; }
    public string Name { get; set; }
    public int? Age { get; set; }
    public IEnumerable<MyOrder> Orders { get; set; }
}