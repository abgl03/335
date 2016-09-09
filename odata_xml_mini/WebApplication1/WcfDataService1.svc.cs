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

namespace WebApplication1 {
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class WcfDataService1 : DataService< MyDataSource > {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config) {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            // config.SetEntitySetAccessRule("MyEntityset", EntitySetRights.AllRead);
            // config.SetEntitySetAccessRule("MyCustomers", EntitySetRights.AllRead);
            // config.SetEntitySetAccessRule("MyOrders", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            // config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
            config.UseVerboseErrors = true;
        }
    }

    [DataServiceKey("CustomerID")]
    public class MyCustomer {
        public string CustomerID { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public IEnumerable<MyOrder> MyOrders { get; set; }
    }

    [DataServiceKey("OrderID")]
    public class MyOrder {
        public string OrderID { get; set; }
        public string CustomerID { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Items { get; set; }
        public MyCustomer MyCustomer { get; set; }
    }

    public class MyDataSource {
        static MyDataSource() {
            _MyOrders =
                XElement.Load(@"C:\usertmp\XMyOrders.xml")
                .Elements("MyOrder")
                .Select(x => new MyOrder {
                    OrderID = (string)x.Element("OrderID"),
                    CustomerID = (string)x.Element("CustomerID"),
                    Description = (string)x.Element("Description"),
                    Price = (decimal)x.Element("Price"),
                    Items = (int)x.Element("Items"),
                }).ToArray();

            _MyCustomers =
                XElement.Load(@"C:\usertmp\XMyCustomers.xml")
                .Elements("MyCustomer")
                .Select(x => new MyCustomer {
                    CustomerID = (string)x.Element("CustomerID"),
                    Name = (string)x.Element("Name"),
                    //Age = (string)x.Element("Age") != "" ? (int?)x.Element("Age") : null,
                    Age = string.IsNullOrEmpty((string)x.Element("Age")) ? null : (int?)x.Element("Age"),
                }).ToArray();

            var _os = _MyOrders.ToLookup(o => o.CustomerID);
            var _cs = _MyCustomers.ToDictionary(c => c.CustomerID);
            foreach (var o in _MyOrders) o.MyCustomer = _cs[o.CustomerID];
            foreach (var c in _MyCustomers) c.MyOrders = _os[c.CustomerID];
        }

        static IEnumerable<MyCustomer> _MyCustomers;
        static IEnumerable<MyOrder> _MyOrders;
        
        public IQueryable<MyCustomer> MyCustomers { get { return _MyCustomers.AsQueryable(); } }
        public IQueryable<MyOrder> MyOrders { get { return _MyOrders.AsQueryable(); } }
    }
}

// http://localhost:8085/WcfDataService1.svc/MyOrders?$expand=MyCustomers&$format=json

// http://localhost:8085/WcfDataService1.svc/MyCustomers?$expand=MyOrders&$format=json
