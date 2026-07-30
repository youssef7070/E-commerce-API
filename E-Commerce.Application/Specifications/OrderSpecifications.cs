using E_Commerce.Domain.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    public class OrderSpecifications:BaseSpecifications<Order, Guid>
    {

        public OrderSpecifications(string email):base( o=>o.BuyerEmail==email)
        {

            AddInclude(o => o.DeliveryMethod);

            AddInclude(o => o.Items);

            AddOrderByDescending(o => o.OrderDate);

        }


        public OrderSpecifications( Guid id , string emial ):base(o => o.Id==id && o.BuyerEmail== emial)
        {

            AddInclude(o => o.DeliveryMethod);

            AddInclude(o => o.Items);

        }



    }



}
