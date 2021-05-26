using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;
using System.Linq;

namespace xUnitTest
{

    public sealed class SealedClass
    {
        delegate bool CheckNum(int x);
        public void PublicMethod()
        {
            Debug.WriteLine("This is public method");
        }

       
        public  void PublicStaticMethod()
        {
            object t = "this is a test";
            Debug.WriteLine("This is public static method");

            var l = new List<int>();
            l.Add(1);
            l.Add(10);
            l.Add(9);
            l.Add(11);

            var b = l.Where((x) =>  LimitNum(x)).ToList();

            var checkNum = new CheckNum(LimitNum);

            var r = checkNum(11);

            Debug.WriteLine(b);


        }

        private bool LimitNum(int max)
        {
            return max < 10;
        }
    }


    public class TestAccessModifier
    {

       [Fact]
       public void TestIt()
        {
            var x = new SealedClass();
            x.PublicMethod();
            x.PublicStaticMethod();

          
        }
     

    }
}
