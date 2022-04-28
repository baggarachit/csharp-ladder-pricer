// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;
using System;

namespace MyApp // Note: actual namespace depends on the project name.
{
    class Program
    {
        private String fix;  // raw fix
        private int maxQuote; // the maximum requested amount till now.
        private List<double> dp; // array to store all the prices. dp[i] denotes the best price for i million size
        private Double[] finalDenoms; // available sizes
        private Double[] finalPrices; // corresponding prices

        // parse the fix and get finalDenoms and finalPrices
        public int Split_m(string txt, ref List<string> strs, char ch)
        {

            int pos = txt.IndexOf(ch);

            int initialPos = 0;

            strs.Clear();
            // Console.WriteLine(pos);
            while (pos != -1)
            {

                strs.Add(txt.Substring(initialPos, pos - initialPos));

                initialPos = pos + 1;

                pos = txt.IndexOf(ch, initialPos);
            }

            if(pos!=-1) strs.Add(txt.Substring(initialPos, Math.Min(pos, txt.Length) - initialPos + 1));
            else strs.Add(txt.Substring(initialPos, txt.Length - initialPos));
            // Console.WriteLine(txt.Substring(initialPos, Math.Min(pos, txt.Length) - initialPos + 1));
            // Console.WriteLine(strs[0]);

            return strs.Count;
        }
        public void parseFix(String f)
        {
            // Console.WriteLine("wjbw");
            fix = f;
            // Console.WriteLine(fix);
            List<string> fixAr = new List<string> { };
            Split_m(fix,ref fixAr,'\u0001');
            // Console.WriteLine("wjbw");
            // Console.WriteLine(fixAr[0]);
            List<Double> denomList = new List<Double>();
            List<Double> priceList = new List<Double>();

            // iterate over fix and get the denoms and prices
            for (int i = 0; i < fixAr.Count; i++)
            {
                // Console.WriteLine("wknbsx");
                string pattern = "[\\p{Cc}\\p{Cf}\\p{Co}\\p{Cn}]";
                fixAr[i] = Regex.Replace(fixAr[i], pattern, "");
                List<string> y = new List<string> { };
                Split_m(fixAr[i],ref y,'=');
                // Console.WriteLine(y[1]);
                if (String.Equals(y[0], "135") && Convert.ToDouble(y[1]) % 1000000 == 0)
                {
                    // Console.WriteLine("wowo");
                    denomList.Add(Convert.ToDouble(y[1]) / 1000000);
                    priceList.Add(Convert.ToDouble(fixAr[i - 2].Split("=")[1]));
                }
            }
            finalDenoms = denomList.ToArray();
            finalPrices = priceList.ToArray();
            // finalPrices = priceList.ToArray(new Double[1]);

            // initialise the prices. They are initialised to the corresponding prices in the fix
            dp = new List<Double>(finalDenoms.Length);
            // dp = new List(finalDenoms.Length);
            dp.Add(finalPrices[0]);
            dp.Add(finalPrices[0]);
            for (int i = 1; i < finalDenoms.Length; i++)
            {
                for (double j = finalDenoms[i - 1] + 1; j <= finalDenoms[i]; j++)
                {
                    dp.Add(finalPrices[i]);
                }
            }

            maxQuote = 1;
            // for(int i=0;i<finalDenoms.Length;i++){
            //     Console.WriteLine(finalDenoms[i]);
            //     Console.WriteLine(finalPrices[i]);
            // }
        }




// get the Best Price for the requested size
public double getBestPrice(double notional)
        {
            int siz = (int)Math.Ceiling(notional);
            // Console.WriteLine(siz);
            if(siz<=maxQuote){
                return dp[siz];
            }
            for(int i=maxQuote+1;i<=siz;i++){
                dp.Add(1000000000000000);
                // Console.WriteLine(dp[i]);
            }
            for(int i=maxQuote+1;i<=siz;i++){
                for(int j=1;j<=finalDenoms[(int)finalDenoms.Length-1];j++){
                    if(i<j) continue;
                    double val = (dp[i - j] * (i - j) + dp[j] * j) / i;
                    dp[i]=Math.Min(dp[i],val);
                }
            }
            maxQuote=siz;
            return dp[siz];
        }
        public static void Main(String[] args)
        {
            // Console.WriteLine("wjbw");
            string fix= "133=1.134698134=1000000135=1000000133=1.134698134=3000000135=3000000133=1.134908134=7000000135=7000000133=1.003134=15000000135=15000000133=1.134918134=20000000135=20000000";
            // // parseFix(fix);
            Program obj = new Program();
            obj.parseFix(fix);
            for (int i = 1; i < 20; i++) {
            Console.WriteLine("Best Ladder Price at Notional(in M) = {0} is : {1} \n", i , obj.getBestPrice((double)i));
            }
            // Console.WriteLine("wjbw");
        }
    }
}