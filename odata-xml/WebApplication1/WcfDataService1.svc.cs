//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using System.Xml.Linq;

namespace WebApplication1
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class WcfDataService1 : DataService<MyDataSource>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
            config.UseVerboseErrors = true;
        }
    }
}
[DataServiceKey("CustomerID")]
public class Customer
{
    public string CustomerID { get; set; }
    public string CompanyName { get; set; }
    public string ContactName { get; set; }
    public IEnumerable<Order> Orders { get; set; }
}

[DataServiceKey("OrderID")]
public class Order
{
    public string OrderID { get; set; }
    public string CustomerID { get; set; }
    public int? EmployeeID { get; set; }
    public string OrderDate { get; set; }
    public string ShippedDate { get; set; }
    public decimal Freight { get; set; }
    public string ShipName { get; set; }
    public string ShipCity { get; set; }
    public string ShipCountry { get; set; }
    public Customer Customer { get; set; }
    public Employee Employee { get; set; }
}

[DataServiceKey("EmployeeID")]
public class Employee
{
    public int EmployeeID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public IEnumerable<Order> Orders { get; set; }
}

public class MyDataSource
{
    static MyDataSource()
    {
        _Orders =
            XElement.Load(@"C:\usertmp\XOrders.xml")
            .Elements("Order")
            .Select(x => new Order
            {
                OrderID = (string)x.Element("OrderID"),
                CustomerID = (string)x.Element("CustomerID"),
                EmployeeID = string.IsNullOrEmpty((string)x.Element("EmployeeID")) ? null : (int?)x.Element("EmployeeID"),
                OrderDate = (string)x.Element("OrderDate"),
                ShippedDate = (string)x.Element("ShippedDate"),
                Freight = (decimal)x.Element("Freight"),
                ShipName = (string)x.Element("ShipName"),
                ShipCity = (string)x.Element("ShipCity"),
                ShipCountry = (string)x.Element("ShipCountry"),
            }).ToArray();

        _Customers =
            XElement.Load(@"C:\usertmp\XCustomers.xml")
            .Elements("Customer")
            .Select(x => new Customer
            {
                CustomerID = (string)x.Element("CustomerID"),
                CompanyName = (string)x.Element("CompanyName"),
                ContactName = (string)x.Element("ContactName"),
            }).ToArray();

        _Employees =
            XElement.Load(@"C:\usertmp\XEmployees.xml")
            .Elements("Employee")
            .Select(x => new Employee
            {
                EmployeeID = (int)x.Element("EmployeeID"),
                FirstName = (string)x.Element("FirstName"),
                LastName = (string)x.Element("LastName"),
            }
            ).ToArray();

        var _os = _Orders.ToLookup(o => o.CustomerID);
        var _cs = _Customers.ToDictionary(c => c.CustomerID);
        var _oe = _Orders.ToLookup(o => o.EmployeeID);
        var _oes = _Employees.ToDictionary(e => e.EmployeeID);
        _oes.Add(0, null);

        foreach (var o in _Orders) o.Customer = _cs[o.CustomerID];
        foreach (var c in _Customers) c.Orders = _os[c.CustomerID];
        foreach (var oe in _Orders) oe.Employee = _oes[oe.EmployeeID.GetValueOrDefault()];
        foreach (var e in _Employees)  e.Orders = _oe[e.EmployeeID]; 

    }

    static IEnumerable<Customer> _Customers;
    static IEnumerable<Order> _Orders;
    static IEnumerable<Employee> _Employees;

    public IQueryable<Customer> Customers { get { return _Customers.AsQueryable(); } }
    public IQueryable<Order> Orders { get { return _Orders.AsQueryable(); } }
    public IQueryable<Employee> Employees { get { return _Employees.AsQueryable(); } }
}


// http://localhost:8085/WcfDataService1.svc/Orders?$expand=Customers&$format=json

// http://localhost:8085/WcfDataService1.svc/Customers?$expand=Orders&$format=json

// http://localhost:8085/WcfDataService1.svc/Employees?$expand=Employees&$format=json