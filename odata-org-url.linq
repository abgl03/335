<Query Kind="Program">
  <Connection>
    <ID>28854290-2192-4aa9-8ec8-fa418c918e4d</ID>
    <Persist>true</Persist>
    <Driver>AstoriaAuto</Driver>
    <Server>http://services.odata.org/Northwind/Northwind.svc/</Server>
  </Connection>
</Query>

void Main() {
    var nwd = this;
    
    var urls = new [] {
        "http://services.odata.org/Northwind/Northwind.svc/Orders()?$orderby=OrderID&$skip=0&$top=10&$expand=Customer,Employee,Order_Details&$select=OrderID,Customer/CompanyName,Customer/ContactName,Employee/FirstName,Employee/LastName,Order_Details/",
        "http://services.odata.org/Northwind/Northwind.svc/Orders()?$orderby=Customer/CompanyName desc,OrderID desc&$skip=0&$top=10&$expand=Customer,Employee,Order_Details&$select=OrderID,Customer/CompanyName,Customer/ContactName,Employee/FirstName,Employee/LastName,Order_Details/",
        "http://services.odata.org/Northwind/Northwind.svc/Orders()?$orderby=Customer/ContactName,OrderID&$skip=40&$top=10&$expand=Customer,Employee,Order_Details&$select=OrderID,Customer/CompanyName,Customer/ContactName,Employee/FirstName,Employee/LastName,Order_Details/",
        "http://services.odata.org/Northwind/Northwind.svc/Orders()?$orderby=Customer/ContactName,OrderID&$skip=3&$top=3&$expand=Customer,Employee,Order_Details&$select=OrderID,Customer/CompanyName,Customer/ContactName,Employee/FirstName,Employee/LastName,Order_Details/",
        "http://services.odata.org/Northwind/Northwind.svc/Orders()?$orderby=Employee/FirstName,Employee/LastName,OrderID&$skip=0&$top=10&$expand=Customer,Employee,Order_Details&$select=OrderID,Customer/CompanyName,Customer/ContactName,Employee/FirstName,Employee/LastName,Order_Details/",
    };
    
    foreach (var url in urls) { 
        var Ods1 = (QueryOperationResponse<Order>)
                    nwd.Execute<Order>(new Uri(url));  
        
        var Ods2 = Ods1.AsEnumerable()
            .Select(ods => new MyModel {
                OrderID = ods.OrderID,
                CompanyName = ods.Customer.CompanyName,
                ContactName = ods.Customer.ContactName,
                EmployeeName = ods.Employee.FirstName + " " + ods.Employee.LastName,
                TotalQuantity = ods.Order_Details.Sum(od => od.Quantity),
                TotalPrice = ods.Order_Details.Sum(od => od.Quantity * od.UnitPrice)
            });
            
        Ods2.Dump("ODATA WebGrid"); 
    }
}

public class MyModel {
    public int OrderID { get; set; }
    public string CompanyName { get; set; }
    public string ContactName { get; set; }
    public string EmployeeName { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalPrice { get; set; }
}

