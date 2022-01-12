using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Nethereum.Web3;
using Nethereum.Contracts;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Nethereum.Uniswap.Contracts.UniswapV2Factory;
using Nethereum.Uniswap.Contracts.UniswapV2Pair;
using Nethereum.Uniswap.Contracts.UniswapV2Pair.ContractDefinition;
using Nethereum.Uniswap.Contracts.UniswapV2Router02;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using Nethereum.Uniswap.Contracts.UniswapV2Router02.ContractDefinition;

namespace BotWpf
{
    public class CustomLabel : Label
    {
        public event EventHandler ContentChanged;

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (ContentChanged != null)
                ContentChanged(this, EventArgs.Empty);
        }
    }
    internal class BotHandler
    {
        public static string bnbcontrac = "0xae13d989dac2f0debff460ac112a837c89baa7cd";
       public static string busdcontrac= "0x7ef95a0fee0dd31b22626fa2e10ee6a223f8a684";
        public static  string usdtContract= "0x7ef95a0fee0dd31b22626fa2e10ee6a223f8a684";
  
        public static async Task<BigDecimal> TokenValueTask(string token, string bnb)
        {
            decimal priceUsd = 0;
    
            var web3 = new Web3(Properties.Settings.Default.BSCNODE);
            int precision = 10000;

            var contractABI = "[ { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Approval\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Transfer\", \"type\": \"event\" }, { \"constant\": true, \"inputs\": [ { \"internalType\": \"address\", \"name\": \"_owner\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" } ], \"name\": \"allowance\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": false, \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"approve\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"payable\": false, \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"constant\": true, \"inputs\": [ { \"internalType\": \"address\", \"name\": \"account\", \"type\": \"address\" } ], \"name\": \"balanceOf\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": true, \"inputs\": [], \"name\": \"decimals\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": true, \"inputs\": [], \"name\": \"getOwner\", \"outputs\": [ { \"internalType\": \"address\", \"name\": \"\", \"type\": \"address\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": true, \"inputs\": [], \"name\": \"name\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": true, \"inputs\": [], \"name\": \"symbol\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": true, \"inputs\": [], \"name\": \"totalSupply\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": false, \"inputs\": [ { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transfer\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"payable\": false, \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"constant\": false, \"inputs\": [ { \"internalType\": \"address\", \"name\": \"sender\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transferFrom\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"payable\": false, \"stateMutability\": \"nonpayable\", \"type\": \"function\" } ]";



            BigDecimal ratio = 0;
            int decimales = 18;
            int decimals2 = 0;
            var contract = web3.Eth.GetContract(contractABI, token);
            var contract2 = web3.Eth.GetContract(contractABI, bnb);


            UniswapV2Router02Service pkswap = new UniswapV2Router02Service(web3, MainWindow.currentRouter);
            try
            {
                {
                    decimales = await contract.GetFunction("decimals").CallAsync<int>();
                    decimals2 = await contract2.GetFunction("decimals").CallAsync<int>();
                    ratio = BigDecimal.Pow(10, decimals2 - decimales);
                    var Path =new List<string>() {token, bnb };
                    var GetAmountsOut = await pkswap.GetAmountsOutQueryAsync(precision, Path, null);
                    
                    priceUsd = (decimal)(GetAmountsOut.Last() / ratio / precision);
                }
           
            
            }
            catch (Exception e)
            {
               await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + "No price for token "+DateTime.Now.TimeOfDay, Colors.Red);
                }), DispatcherPriority.Render);
            }
            return priceUsd;
        }
        public static async Task<BigInteger> SlippageTask(string from, string to,string amount)
        {
            BigInteger priceUsd = 0;

            var web3 = new Web3(Properties.Settings.Default.BSCNODE);
            int precision = 10000000;

            var contractABI = "[ { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Approval\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Transfer\", \"type\": \"event\" }, { \"constant\": true, \"inputs\": [ { \"internalType\": \"address\", \"name\": \"_owner\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" } ], \"name\": \"allowance\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": false, \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"approve\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"payable\": false, \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"constant\": true, \"inputs\": [ { \"internalType\": \"address\", \"name\": \"account\", \"type\": \"address\" } ], \"name\": \"balanceOf\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": true, \"inputs\": [], \"name\": \"decimals\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": true, \"inputs\": [], \"name\": \"getOwner\", \"outputs\": [ { \"internalType\": \"address\", \"name\": \"\", \"type\": \"address\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": true, \"inputs\": [], \"name\": \"name\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": true, \"inputs\": [], \"name\": \"symbol\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": true, \"inputs\": [], \"name\": \"totalSupply\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"payable\": false, \"stateMutability\": \"view\", \"type\": \"function\" }, { \"constant\": false, \"inputs\": [ { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transfer\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"payable\": false, \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"constant\": false, \"inputs\": [ { \"internalType\": \"address\", \"name\": \"sender\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transferFrom\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"payable\": false, \"stateMutability\": \"nonpayable\", \"type\": \"function\" } ]";



            BigDecimal ratio = 0;
            int decimales = 18;
            int decimals2 = 0;
            var contract = web3.Eth.GetContract(contractABI, from);
            var contract2 = web3.Eth.GetContract(contractABI, to);


            UniswapV2Router02Service pkswap = new UniswapV2Router02Service(web3, MainWindow.currentRouter);
            try
            {
                {
                    var Path = new List<string>();
                    decimales = await contract.GetFunction("decimals").CallAsync<int>();
                    decimals2 = await contract2.GetFunction("decimals").CallAsync<int>();
                    ratio = BigDecimal.Pow(10, decimals2 - decimales);
                  
                         Path = new List<string>() { from, to };

             
                
                     
                    var GetAmountsOut = await pkswap.GetAmountsOutQueryAsync(Web3.Convert.ToWei(amount,UnitConversion.EthUnit.Ether), Path, null);

                    priceUsd = (GetAmountsOut.Last());
                }


            }
            catch (Exception e)
            {
                await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + "Slippage failed may not have price yet", Colors.Red);
                }), DispatcherPriority.Render);
            }
            return priceUsd;
        }
      
        public static async Task<string> GetNameTask(string token)
        {
            var nameFunction = "";
            var contractABI =
                      @"[{""inputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""owner"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""spender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""Approval"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""Burn"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""}],""name"":""Mint"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0In"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1In"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0Out"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1Out"",""type"":""uint256""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""Swap"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""uint112"",""name"":""reserve0"",""type"":""uint112""},{""indexed"":false,""internalType"":""uint112"",""name"":""reserve1"",""type"":""uint112""}],""name"":""Sync"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""from"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""Transfer"",""type"":""event""},{""constant"":true,""inputs"":[],""name"":""DOMAIN_SEPARATOR"",""outputs"":[{""internalType"":""bytes32"",""name"":"""",""type"":""bytes32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""MINIMUM_LIQUIDITY"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""PERMIT_TYPEHASH"",""outputs"":[{""internalType"":""bytes32"",""name"":"""",""type"":""bytes32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""allowance"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""spender"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""approve"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""burn"",""outputs"":[{""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""decimals"",""outputs"":[{""internalType"":""uint8"",""name"":"""",""type"":""uint8""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""factory"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getReserves"",""outputs"":[{""internalType"":""uint112"",""name"":""_reserve0"",""type"":""uint112""},{""internalType"":""uint112"",""name"":""_reserve1"",""type"":""uint112""},{""internalType"":""uint32"",""name"":""_blockTimestampLast"",""type"":""uint32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""_token0"",""type"":""address""},{""internalType"":""address"",""name"":""_token1"",""type"":""address""}],""name"":""initialize"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""kLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""mint"",""outputs"":[{""internalType"":""uint256"",""name"":""liquidity"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""name"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""nonces"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""owner"",""type"":""address""},{""internalType"":""address"",""name"":""spender"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""deadline"",""type"":""uint256""},{""internalType"":""uint8"",""name"":""v"",""type"":""uint8""},{""internalType"":""bytes32"",""name"":""r"",""type"":""bytes32""},{""internalType"":""bytes32"",""name"":""s"",""type"":""bytes32""}],""name"":""permit"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""price0CumulativeLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""price1CumulativeLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""skim"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""uint256"",""name"":""amount0Out"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""amount1Out"",""type"":""uint256""},{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""bytes"",""name"":""data"",""type"":""bytes""}],""name"":""swap"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""symbol"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""sync"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""token0"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""token1"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalSupply"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""transfer"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""from"",""type"":""address""},{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""transferFrom"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""}]";

            var web3 = new Web3(Properties.Settings.Default.BSCNODE);
            var contract = web3.Eth.GetContract(contractABI, token);
            try
            {
                 nameFunction = await contract.GetFunction("symbol").CallAsync<string>();
                 if (nameFunction == null)
                 {
                     nameFunction = "Token";
                 }
            }
            catch (Exception e)
            {
                
            }
            
            return nameFunction;
        }
        


         public static async Task<decimal> GetAccountBalance()
         {

             double value = 0;
             var web3 = new Web3(Properties.Settings.Default.BSCNODE);
             var balance =
                 await web3.Eth.GetBalance.SendRequestAsync(Properties.Settings.Default.Wallet);
             value = (double) balance.Value;
             return  Web3.Convert.FromWei(balance.Value);
   

         }
        public static async Task<TxResult> DeBNBaToken(string cantidad, decimal minimo, List<string> tokens, string gas)
        {
            try
            {
                var url = Properties.Settings.Default.BSCNODE;
                var myWallet = Properties.Settings.Default.Wallet;
                var privateKey = Properties.Settings.Default.PK;// Properties.Settings.Default.PK;
                var account = new Account(privateKey, 97);
                var web3 = new Web3(account, url);
                var camtidadBUSD = Web3.Convert.ToWei(cantidad,UnitConversion.EthUnit.Ether);// Web3.Convert.ToWei(decimal.Parse(cantidad));//CANTIDAD DE BUSD A INTERCAMBIAR - 20$
                var camtidadToken = Web3.Convert.ToWei(minimo,UnitConversion.EthUnit.Ether);//MINIMO DE TOKEN META A RECIBIR - 5 META
                var uniswapV2Router02Service = new UniswapV2Router02Service(web3, MainWindow.currentRouter);
                var deadline = DateTimeOffset.Now.AddMinutes(15).ToUnixTimeSeconds();
                var swapEthForExactTokens = new Nethereum.Uniswap.Contracts.UniswapV2Router02.ContractDefinition.SwapExactETHForTokensSupportingFeeOnTransferTokensFunction()
                {
                    AmountOutMin = camtidadToken,
                    Path = tokens,
                    Deadline = deadline,
                    To = myWallet,
                    AmountToSend = camtidadBUSD,
                    GasPrice = Web3.Convert.ToWei(gas, UnitConversion.EthUnit.Gwei),
                    Gas = BigInteger.Parse("300000")
                };

                var swapReceipt = await uniswapV2Router02Service.SwapExactETHForTokensSupportingFeeOnTransferTokensRequestAndWaitForReceiptAsync(swapEthForExactTokens);
                var swapLog = swapReceipt.Logs.DecodeAllEvents<SwapEventDTO>();
                var resultado = new TxResult();
                var result = await Task.Run(async () => { return swapReceipt.Status.HexValue;}) ;
                  if (result == "0x1")
                  {
                      decimal temp =0;
                      resultado = new TxResult();
                      
                      resultado.txHash = swapReceipt.TransactionHash;
                      resultado.Time=DateTime.Now;
                      
                      resultado.result = "Success";
                      if (swapLog.Count == 1)
                      { temp = Web3.Convert.FromWei(swapLog[0].Event.Amount0Out);
                         
                          resultado.value = temp.ToString();
                          temp = Web3.Convert.FromWei(swapLog[0].Event.Amount1In);
                          resultado.ValueSpend = temp.ToString();
                          await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                          MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + "Success Tx: " + swapReceipt.TransactionHash + "Value: " + Web3.Convert.FromWei(swapLog[0].Event.Amount1In).ToString(), Colors.Green);
                      }), DispatcherPriority.Render);
                      }
                      else
                      {
                          if (swapLog.Count == 2)
                          {
                              temp = Web3.Convert.FromWei(swapLog[1].Event.Amount1Out);

                              resultado.value = temp.ToString();
                              temp = Web3.Convert.FromWei(swapLog[0].Event.Amount1In);
                              resultado.ValueSpend = temp.ToString();
                            await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                                  MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + "Success Tx: " + swapReceipt.TransactionHash + "Value: " + Web3.Convert.FromWei(swapLog[0].Event.Amount1In).ToString(), Colors.Green);
                              }), DispatcherPriority.Render);
                        }
                          else
                          {
                              await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                              MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + "Success Tx: " + swapReceipt.TransactionHash + "Value: "  , Colors.Green);
                          }), DispatcherPriority.Render);
                          }
                          
                    }
                    return resultado;

                  }
                  else
                  {
       
                    resultado = new TxResult();
                    resultado.value = "0";
                    resultado.txHash = swapReceipt.TransactionHash;
                    resultado.Time = DateTime.Now;
                    resultado.ValueSpend = "0";
                    resultado.result = "Failed";
                    await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + "Failed Tx: " + swapReceipt.TransactionHash , Colors.Red);
                    }), DispatcherPriority.Render);
                    return resultado;
                }

            }
            catch (Exception ex)
            {
               await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + ex.Message + Environment.NewLine + "Contact support", Colors.Red);
                }), DispatcherPriority.Render);
                return
                    new TxResult(); 
            }
            
        }

        
        public static async Task<TxResult> AproveTask(string token, string gas)
        {
            try
            {
                TxResult result = new TxResult();
                var account = new Account(Properties.Settings.Default.PK, 97);
                var web3 = new Web3(account, Properties.Settings.Default.BSCNODE);
                var contract = web3.Eth.GetContractTransactionHandler<ApproveFunction>();
                var approve = new ApproveFunction()
                {
                    Spender = MainWindow.currentRouter,
                    Value = BigInteger.Pow(2, 256) - 1,
                    Gas = 50000,
                    GasPrice = Web3.Convert.ToWei(gas, UnitConversion.EthUnit.Gwei)
                };
                var result2 = await contract.SendRequestAndWaitForReceiptAsync(token, approve);

                var status = await Task.Run(async () => { return result2.Status.HexValue; });
                if (status == "0x1")
                {
                    result.result = "Success";
                    result.txHash = result2.TransactionHash;
                    result.Time = DateTime.Now;
                    await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + "Success Tx: " + result2.TransactionHash, Colors.Green);
                    }), DispatcherPriority.Render);
                }
                else
                {
                    result.result = "Failed";
                    result.txHash = result2.TransactionHash;
                    result.Time = DateTime.Now;
                    await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + "Failed Tx: " + result2.TransactionHash, Colors.Red);
                    }), DispatcherPriority.Render);
                }
                return result;
            }
            catch (Exception e)
            {
                await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + e.Message + Environment.NewLine + "Contact support", Colors.Red);
                }), DispatcherPriority.Render);
                return new TxResult();
            }
           
        }

        public static async Task<TxResult> DeTokenABNB(string cantidad, decimal minimo, List<string> tokens, string gas)
        {
            try
            {
                var url = Properties.Settings.Default.BSCNODE;
                var myWallet = Properties.Settings.Default.Wallet;
                var privateKey = Properties.Settings.Default.PK;
                var account = new Account(privateKey, 97);
                var web3 = new Web3(account, url);
                var uniswapV2Router02Service = new UniswapV2Router02Service(web3, MainWindow.currentRouter);
                var camtidadBUSD = Web3.Convert.ToWei(cantidad,UnitConversion.EthUnit.Ether);//CANTIDAD DE BUSD A INTERCAMBIAR - 20$
                var camtidadToken = Web3.Convert.ToWei(minimo, UnitConversion.EthUnit.Ether);//MINIMO DE TOKEN META A RECIBIR - 5 META
                var swapDTO = new SwapExactTokensForETHSupportingFeeOnTransferTokensFunction()
                {
                    AmountIn = camtidadBUSD,
                    AmountOutMin = camtidadToken, //MINIMO DE TOKENS A RECIBIR
                    Path = tokens,
                    To = myWallet,
                    Deadline = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds() + 260,
                    GasPrice = Web3.Convert.ToWei(gas, UnitConversion.EthUnit.Gwei),
                    Gas = 300000
                };
                var swapReceipt = await uniswapV2Router02Service.SwapExactTokensForETHSupportingFeeOnTransferTokensRequestAndWaitForReceiptAsync(swapDTO);
                var swapLog = swapReceipt.Logs.DecodeAllEvents<SwapEventDTO>();
                var resultado = new TxResult();
                var result = await Task.Run(async () => { return swapReceipt.Status.HexValue; });
                if (result == "0x1")
                {
                    decimal temp = 0;
                    resultado = new TxResult();
                   
                    resultado.txHash = swapReceipt.TransactionHash;
                    resultado.Time = DateTime.Now;
                    
                    resultado.result = "Success";
                    if (swapLog.Count == 1)
                    {
                         temp = Web3.Convert.FromWei(swapLog[0].Event.Amount1Out);
                        resultado.value = temp.ToString();
                        temp = Web3.Convert.FromWei(swapLog[0].Event.Amount0In);
                        resultado.ValueSpend = temp.ToString();
                        await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                            MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + "Success Tx: " + swapReceipt.TransactionHash + "Value: " + Web3.Convert.FromWei(swapLog[0].Event.Amount1In).ToString(), Colors.Green);
                        }), DispatcherPriority.Render);
                        return resultado;
                    }
                    else
                    {
                        if (swapLog.Count == 2)
                        {
                            temp = Web3.Convert.FromWei(swapLog[1].Event.Amount1Out);
                            resultado.value = temp.ToString();
                            temp = Web3.Convert.FromWei(swapLog[0].Event.Amount1In);
                            resultado.ValueSpend = temp.ToString();
                            await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                                MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + "Success Tx: " + swapReceipt.TransactionHash + "Value: " + Web3.Convert.FromWei(swapLog[1].Event.Amount1Out).ToString(), Colors.Green);
                            }), DispatcherPriority.Render);
                            return resultado;
                        }
                        else
                        {
                            await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                                MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + "Success Tx: " + swapReceipt.TransactionHash + "Value: ", Colors.Green);
                            }), DispatcherPriority.Render);
                            return resultado;
                        }
                    }
                   
                }
                else
                {
                    resultado = new TxResult();
                    resultado.txHash = swapReceipt.TransactionHash;
                    resultado.Time = DateTime.Now;
                    resultado.result = "Failed";
                    if (swapLog.Count > 0)
                    {
                        resultado.value = swapLog[0].Event.Amount1In.ToString();
                        resultado.ValueSpend = swapLog[0].Event.Amount0Out.ToString();
                    }
                    await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + "Failed Tx: " + swapReceipt.TransactionHash , Colors.Red);
                    }), DispatcherPriority.Render);
                    return resultado;
                }
            }
            catch (Exception ex)
            {
               await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    MainWindow.Instance.Consola1.WriteOutput(Environment.NewLine + ex.Message + Environment.NewLine + "Contact support", Colors.Red);
                }), DispatcherPriority.Render);
                return new TxResult();
            }

            
        }
        public static async Task<decimal> TokenBalanceAsync(string token)
           {
               var web3 = new Web3(Properties.Settings.Default.BSCNODE);

               if (token.Length != 42)
                   return 0;
               var contractABI =
                   @"[{""inputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""owner"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""spender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""Approval"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""Burn"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""}],""name"":""Mint"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0In"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1In"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0Out"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1Out"",""type"":""uint256""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""Swap"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""uint112"",""name"":""reserve0"",""type"":""uint112""},{""indexed"":false,""internalType"":""uint112"",""name"":""reserve1"",""type"":""uint112""}],""name"":""Sync"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""from"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""Transfer"",""type"":""event""},{""constant"":true,""inputs"":[],""name"":""DOMAIN_SEPARATOR"",""outputs"":[{""internalType"":""bytes32"",""name"":"""",""type"":""bytes32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""MINIMUM_LIQUIDITY"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""PERMIT_TYPEHASH"",""outputs"":[{""internalType"":""bytes32"",""name"":"""",""type"":""bytes32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""allowance"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""spender"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""approve"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""burn"",""outputs"":[{""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""decimals"",""outputs"":[{""internalType"":""uint8"",""name"":"""",""type"":""uint8""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""factory"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getReserves"",""outputs"":[{""internalType"":""uint112"",""name"":""_reserve0"",""type"":""uint112""},{""internalType"":""uint112"",""name"":""_reserve1"",""type"":""uint112""},{""internalType"":""uint32"",""name"":""_blockTimestampLast"",""type"":""uint32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""_token0"",""type"":""address""},{""internalType"":""address"",""name"":""_token1"",""type"":""address""}],""name"":""initialize"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""kLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""mint"",""outputs"":[{""internalType"":""uint256"",""name"":""liquidity"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""name"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""nonces"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""owner"",""type"":""address""},{""internalType"":""address"",""name"":""spender"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""deadline"",""type"":""uint256""},{""internalType"":""uint8"",""name"":""v"",""type"":""uint8""},{""internalType"":""bytes32"",""name"":""r"",""type"":""bytes32""},{""internalType"":""bytes32"",""name"":""s"",""type"":""bytes32""}],""name"":""permit"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""price0CumulativeLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""price1CumulativeLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""skim"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""uint256"",""name"":""amount0Out"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""amount1Out"",""type"":""uint256""},{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""bytes"",""name"":""data"",""type"":""bytes""}],""name"":""swap"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""symbol"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""sync"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""token0"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""token1"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalSupply"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""transfer"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""from"",""type"":""address""},{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""transferFrom"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""}]";
               try
               {
                   var contract4 = web3.Eth.GetContract(contractABI, token);
                   var balanceFunction = contract4.GetFunction("balanceOf");
                   var balance4 = await balanceFunction.CallAsync<BigInteger>(Properties.Settings.Default.Wallet);
                   var decimales = await contract4.GetFunction("decimals").CallAsync<int>();
                   if(decimales==18)
                       return Web3.Convert.FromWei(balance4,UnitConversion.EthUnit.Ether);
                   
                    return Web3.Convert.FromWei(balance4, UnitConversion.EthUnit.Mwei);
                   
               }
               catch (Exception ex)
               {
                   return 0;
               }
           }
       
    }
    internal static class NativeMethods
    {
        // See http://msdn.microsoft.com/en-us/library/ms649021%28v=vs.85%29.aspx
        public const int WM_CLIPBOARDUPDATE = 0x031D;
        public static IntPtr HWND_MESSAGE = new IntPtr(-3);

        // See http://msdn.microsoft.com/en-us/library/ms632599%28VS.85%29.aspx#message_only
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);
    }
    public class ClipboardManager
    {
        public event EventHandler ClipboardChanged;

        public ClipboardManager(Window windowSource)
        {
            HwndSource source = PresentationSource.FromVisual(windowSource) as HwndSource;
            if (source == null)
            {
                throw new ArgumentException(
                    "Window source MUST be initialized first, such as in the Window's OnSourceInitialized handler."
                    , nameof(windowSource));
            }

            source.AddHook(WndProc);

            // get window handle for interop
            IntPtr windowHandle = new WindowInteropHelper(windowSource).Handle;

            // register for clipboard events
            NativeMethods.AddClipboardFormatListener(windowHandle);
        }

        private void OnClipboardChanged()
        {
            ClipboardChanged?.Invoke(this, EventArgs.Empty);
        }

        private static readonly IntPtr WndProcSuccess = IntPtr.Zero;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_CLIPBOARDUPDATE)
            {
                OnClipboardChanged();
                handled = true;
            }

            return WndProcSuccess;
        }
    }

   
}
