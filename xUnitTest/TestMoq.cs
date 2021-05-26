using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace xUnitTest
{
   

    public class Customer: ICustomer, ICustomer2
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Balance { get; set; }
       

    }

    public interface ICustomer
    {
        int Id { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        double Balance { get; set; }
        

    }

    public interface ICustomer2
    {
        int Id { get; set; }
    }

    public class TestMoq
    {
        [Fact]
        public void TestMockCustomer()
        {
            var customer = new Mock<ICustomer>();
            customer.Setup(x => x.FirstName).Returns("Kevin");

         
            Debug.WriteLine(customer.Object.FirstName);
            //Debug.WriteLine(customer.LastName);
            //Debug.WriteLine(customer.Id);
            //Debug.WriteLine(customer.Balance);
            var myarray = new String[] { "one", "2", "three", "4", "really big number", "2324573984927361" };

            //create shallow copy by CopyTo
            //You have to instantiate your new array first
            object[] myarray2 = new object[myarray.Length];
            //but then you can specify how many members of original array you would like to copy 
            myarray.CopyTo(myarray2, 0);

            //create shallow copy by Clone
            object[] myarray1;
            //here you don't need to instantiate array, 
            //but all elements of the original array will be copied
            myarray1 = myarray.Clone() as object[];

            //if not sure that we create a shalow copy lets test it
            myarray[0] = "0";
            Debug.WriteLine(myarray[0]);// print 0
            Array.Sort(myarray1);
            Debug.WriteLine(myarray1[0]);//print "one"
            Debug.WriteLine(myarray2[0]);//print "one"
           

        }
    }
}
