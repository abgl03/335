<Query Kind="Program">
  <Connection>
    <ID>992adde7-5d79-4a30-9dd4-c813768d6fad</ID>
    <Persist>true</Persist>
    <Driver>AstoriaAuto</Driver>
    <Server>http://services.odata.org/Northwind/Northwind.svc/</Server>
  </Connection>
</Query>

void Main() {
    var nwd = this;
    
    var page = 3;
    var rowsPerPage = 10;
    var res = nwd.Orders
        .OrderBy(o => o.OrderID)
        .Skip((page - 1) * rowsPerPage)
        .Take(rowsPerPage)
        .Select(o => new {
            o.OrderID,
            o.Customer.CompanyName,
            EmployeeName = o.Employee.FirstName + " " + o.Employee.LastName,
        });
        
   res.Dump("res");
}