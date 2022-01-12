using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using Nethereum.Util;
using Nethereum.Web3;
using Newtonsoft.Json.Linq;

namespace BotWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private HttpClient _httpClient;

        private readonly string RugdocCheckUrl =
            "https://honeypot.api.rugdoc.io/api/honeypotStatus.js?address={0}&chain=bsc";

        public static string bnbcontrac = "0xae13d989dac2f0debff460ac112a837c89baa7cd";
        public static string busdcontrac = "0x78867bbeef44f2326bf8ddd1941a4439382ef2a7";
        public static string usdtContract = "0x7ef95a0fee0dd31b22626fa2e10ee6a223f8a684";
        public static string panacakSwapRouter = "0x9ac64cc6e4415144c455bd8e4837fea55603e5c3";
        public  static string currentRouter = panacakSwapRouter;
        public ObservableCollection<TxResult> resultsBuy = new ObservableCollection<TxResult>();
        public static decimal tokenBalanceD = 0;
        public static decimal bnbBalanceD = 0;
        public static decimal bnbPrice = 0;
        public static decimal tokenPriceLast = 0;
        public static decimal tokenPriceAmountLast = 0;
        public static decimal tokenPriceBuy = 0;
        public static decimal tokenPriceSell = 0;
        public static MainWindow Instance;
        static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public static bool DoingSomething = false;
     
        
    public MainWindow()
        {
            InitializeComponent();
           
      Instance=this;
            buyTxGrid.ItemsSource = resultsBuy;
            _httpClient = new HttpClient();
            Task.Run(async () => await TokenBalance());
            bscNode.Text = Properties.Settings.Default.BSCNODE;
            pkAddress.Password = Properties.Settings.Default.PK;

            ValDatos();
            
            
        }
        
        protected override void OnSourceInitialized(EventArgs e)
        {
                base.OnSourceInitialized(e);

                // Initialize the clipboard now that we have a window soruce to use
                var windowClipboardManager = new ClipboardManager(this);
                windowClipboardManager.ClipboardChanged += ClipboardChanged;
        }
        private void ClipboardChanged(object sender, EventArgs e)
        {
                // Handle your clipboard update here, debug logging example:
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        var text = Clipboard.GetText();
                        if (autopaste.IsOn)
                        {
                            if (text.IsValidEthereumAddressHexFormat())
                            { tokenBuy.Text = Clipboard.GetText();
                                autopaste.IsOn = false;
                            }

                           
                        }
                    }
                }
                catch (Exception exception)
                {
                    
                }
               
        }
        public async Task<bool> RugdocCheck(string token)
        {
                if (!auditT.IsOn)
                {
                    return true;
                }
                try
                {
                    var response = await _httpClient.GetAsync(string.Format(RugdocCheckUrl, token));
                    var rugdocStr = await response.Content.ReadAsStringAsync();
                    var responseObject = JObject.Parse(rugdocStr);
                    var valid = responseObject["status"].Value<string>().Equals("OK", StringComparison.InvariantCultureIgnoreCase);
                    await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        Consola1.WriteOutput(Environment.NewLine + String.Format(
                            "Rugdoc check token {0} Status: {1} RugDoc Response: {2}", token, valid, rugdocStr), Colors.Red);
                    }), DispatcherPriority.Render);
               
                    return valid;
                }
                catch (Exception e)
                {
                    await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        Consola1.WriteOutput(Environment.NewLine + e.Message+" Contact Support", Colors.Red);
                    }), DispatcherPriority.Render);
                    return false;
                }
        }
            public async void ValDatos()
            {
               
                if ( Properties.Settings.Default.BSCNODE.Length>4 &&
                        Properties.Settings.Default.PK.Length == 66)
                    {
                        buyBtn.IsEnabled = true;
                        sellBtn.IsEnabled = true;
                        sellBtnAll.IsEnabled = true;
                        sellBtnX.IsEnabled = true;
                       
                    }
                    else
                    {
                        Consola1.WriteOutput("Please set account info before continue.",Colors.Red);
                    }
                
                
            }

    
            public async Task TokenBalance()
            {
                decimal bnbBalance = 0;
                while (true)
                {
                    try
                    {
                        var bnbPrice2 = await BotHandler.TokenValueTask(bnbcontrac, busdcontrac);
                    
                        bnbBalance = await BotHandler.GetAccountBalance();
                        await  Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                            bnbBalanceD=bnbBalance;
                        }), DispatcherPriority.Render);
                        var buytoken = "";
                        bool route=false;
                        tokenBuy.Dispatcher.Invoke(
                            DispatcherPriority.Normal,
                            (ThreadStart)delegate { buytoken = fromBuy.Text; });
                        await    Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                            bnbName.Content = "BNB Balance";
                            pairBalance.Content = bnbBalanceD.ToString();
                            bnbPrice =(decimal) bnbPrice2;
                        }), DispatcherPriority.Render);
          
                        var text = "";
                        tokenBuy.Dispatcher.Invoke(
                            DispatcherPriority.Normal,
                            (ThreadStart)delegate { text = tokenBuy.Text; });
                        if (text.IsValidEthereumAddressHexFormat())
                        {
                            decimal value2 = 0;
                            var result = await BotHandler.TokenBalanceAsync(text);
                            BigDecimal value = 0;
                            
                            if (buytoken == "BNB")
                            {
                                value = await BotHandler.TokenValueTask(text, bnbcontrac);
                                if(bnbPrice!=0)
                                    value = (decimal) (value * bnbPrice);
                            }
                            else
                            {
                                if (buytoken == "BUSD")
                                {
                                    value = await BotHandler.TokenValueTask(text, busdcontrac);
                                
                                }
                                else
                                {
                                    if (buytoken == "USDT")
                                    {
                                        value = await BotHandler.TokenValueTask(text, usdtContract);
                                    
                                    }
                                }
                            }
                            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                tokenBalanceD = result;
                                tokenBalance.Text = result.ToString();
                                tokenVl.Content = value.ToString();
                            }), DispatcherPriority.Render);

                    }
                    
                    
                    }
                    catch (Exception e)
                    {
                        await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                            Consola1.WriteOutput(Environment.NewLine + e.Message + " Contact Support", Colors.Red);
                        }), DispatcherPriority.Render);
                    }

                    await Task.Delay(500);
                }
            }
            private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {

            }
            private void Button_Click(object sender, RoutedEventArgs e)
            {

            }

            private void Button_Click_1(object sender, RoutedEventArgs e)
            {

            }

            private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                var value =  (ComboBox)sender;
                Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    tokenPair1.Content =value.Text;
                }), DispatcherPriority.Render);
            }

            private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
            {

            }

            private void buyTxGrid_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {

            }

            private void PreviewTextInput(object sender, TextCompositionEventArgs e)
            {
                Regex regex = new Regex("^[,][0-9]+$|^[0-9]*[,]{0,1}[0-9]*$");
                e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
            }
            private void PreviewTextInputInt(object sender, TextCompositionEventArgs e)
            {
                Regex regex = new Regex("[^0-9.-]+");
                e.Handled = regex.IsMatch(e.Text);
            }

            private async void buyBtn_Click(object sender, RoutedEventArgs e)
            {
                if (ValidarDatosBuy())
                {
                           if (!DoingSomething)
                           {    Buy();
                                DoingSomething = true;
                           }
                }
                else

                {
                    Consola1.WriteOutput("Please look your inputs",Colors.Red);
                }

            }

            public async void Buy()
            {
                decimal slip = 0;
                
                buyBtn.IsEnabled = false;
                sellBtn.IsEnabled = false;
                sellBtnAll.IsEnabled = false;
                sellBtnX.IsEnabled = false;
                List<string> lista = new List<string>();
                
                
                    if (fromBuy.Text == "BNB")
                    {
                        
                       

                        
                            lista.Add(bnbcontrac);
                            lista.Add(tokenBuy.Text);
                            tokenPair1.Foreground = new SolidColorBrush((Colors.Blue));
                            tokenPair1.Content = "BNB";
                            if (!slipMax_Copy.IsOn)
                            {
                                var amount = await BotHandler.SlippageTask(bnbcontrac,
                                    tokenBuy.Text,
                                    amountBuy.Text);

                                slip = ((decimal) (Web3.Convert.FromWei(amount,
                                           UnitConversion.EthUnit.Ether))) *
                                       ((100 - decimal.Parse(SlipBuy.Text)) / 100);
                            }
                        
                        
                    }
                    else
                    {
                        bool pairF = false;
                        if (fromBuy.Text == "BUSD")
                        {

                           

                            
                                lista.Add(bnbcontrac);
                                lista.Add(busdcontrac);
                                lista.Add(tokenBuy.Text);
                                tokenPair1.Foreground = new SolidColorBrush((Colors.Blue));
                                tokenPair1.Content = "BUSD";
                                if (!slipMax_Copy.IsOn)
                                {
                                    var input = decimal.Parse(amountBuy.Text) * bnbPrice;
                                    var amount = await BotHandler.SlippageTask(busdcontrac,
                                        tokenBuy.Text,
                                        input.ToString());
                                    slip = ((decimal) (Web3.Convert.FromWei(amount,
                                               UnitConversion.EthUnit.Ether))) *
                                           ((100 - decimal.Parse(SlipBuy.Text)) / 100);
                                }
                            
                           
                        }
                        else
                        {
                            if (fromBuy.Text == "USDT")
                            {
                               

                                 lista.Add(bnbcontrac);
                                    lista.Add(usdtContract);
                                    lista.Add(tokenBuy.Text);
                                    tokenPair1.Foreground = new SolidColorBrush((Colors.Blue));
                                    tokenPair1.Content = "BUSD";
                                    if (!slipMax_Copy.IsOn)
                                    {
                                        var input = decimal.Parse(amountBuy.Text) * bnbPrice;
                                        var amount = await BotHandler.SlippageTask(usdtContract,
                                            tokenBuy.Text,
                                            input.ToString());
                                        slip = ((decimal) (Web3.Convert.FromWei(amount,
                                                   UnitConversion.EthUnit.Ether))) *
                                               ((100 - decimal.Parse(SlipBuy.Text)) / 100);
                                    }
                              
                            }
                        }
                    }
                    if (lista.Count > 0)
                    {
                        if (await (RugdocCheck(tokenBuy.Text)))
                        {
                            
                                Consola1.WriteOutput(Environment.NewLine + "Buying " + tokenName.Content + " amount: " + amountBuy.Text, Colors.Green);
                               var resultado = await BotHandler.DeBNBaToken(amountBuy.Text, slip, lista, gweiAmount.Text);
                                if (resultado.result == "Success")
                                {
                                    tokenPriceBuy = tokenPriceLast* decimal.Parse(resultado.value);
                                    tokenbuyPrice.Content= tokenPriceBuy.ToString();
                                    resultsBuy.Add(resultado);
                                    if (approveAfter.IsOn)
                                    {
                                        await BotHandler.AproveTask(tokenBuy.Text, gweiAmount.Text);
                                    }
                                    if (autoSellOn.IsOn)
                                    {
                                        DoingSomething = false;
                                        amountSell.Text = resultado.value;
                                        AutosellTask(int.Parse(delay.Text), int.Parse(profitT.Text));
                                    }

                                }
                                
                            
                        }
                    }
                

                DoingSomething = false;
                buyBtn.IsEnabled = true;
                sellBtn.IsEnabled = true;
                sellBtnAll.IsEnabled = true;
                sellBtnX.IsEnabled = true;
            }
 
        public bool ValidarDatosBuy()
            {
                if (amountBuy.Text != "" && gweiAmount.Text != "")
                {
                    if (tokenBuy.Text.IsValidEthereumAddressHexFormat())
                    {
                        if (decimal.Parse(amountBuy.Text) > 0 &&
                            decimal.Parse((string)pairBalance.Content) != decimal.Parse(amountBuy.Text))
                        {
                            if (int.Parse(gweiAmount.Text) >= 5)
                            {
                                if (int.Parse(sellxText.Text) > 0 && int.Parse(sellxText.Text) <= 100)
                                {
                                    if (int.Parse(SlipBuy.Text) > 5 && int.Parse(SlipBuy.Text)<100)
                                    {
                                        if (int.Parse(gweiAmount.Text) > 0 && decimal.Parse(amountBuy.Text) > 0)
                                        {
                                            
                                            return true;

                                        }
                                        else
                                        {
                                            Consola1.WriteOutput(Environment.NewLine + "Set valid amount or gas", Colors.Red);
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        Consola1.WriteOutput(Environment.NewLine + "Slip % incorrect, min 5 max 100", Colors.Red);
                                        return false;
                                    }
                                }
                                else
                                {
                                    Consola1.WriteOutput(Environment.NewLine + "Sell % incorrect", Colors.Red);
                                    return false;
                                }
                            }
                            else
                            {

                                Consola1.WriteOutput(Environment.NewLine + "Error: Gas amount too low or incorrect.",
                                    Colors.Red);
                                return false;

                            }
                        }
                        else
                        {
                            Consola1.WriteOutput(Environment.NewLine + "Error: Amount not enoght or incorrect", Colors.Red);
                            return false;
                        }
                    }
                    else
                    {
                        Consola1.WriteOutput(Environment.NewLine + "Error: Input a valid Token Address", Colors.Red);
                        return false;
                    }
                }
                else
                {
                    Consola1.WriteOutput(Environment.NewLine + "Error: Input a valid Gas and amount", Colors.Red);
                    return false;
                }
            }

            private async void AproveBtn_Click(object sender, RoutedEventArgs e)
            {
                AproveBtn.IsEnabled = false;
                buyBtn.IsEnabled = false;
                sellBtn.IsEnabled = false;
                sellBtnAll.IsEnabled = false;
                sellBtnX.IsEnabled = false;
                if(!DoingSomething)
                    if (tokenBuy.Text.Length == 42)
                    {
                        DoingSomething = true;
                        var result = await BotHandler.AproveTask(tokenBuy.Text, gweiAmount.Text);
                    
                        resultsBuy.Add(result);
                        if (result.result== "Success")
                        {
                            Consola1.WriteOutput(Environment.NewLine+"Contract Approved",Colors.Green);
                          
                        }
                        else
                        {
                        
                            Consola1.WriteOutput(Environment.NewLine + "Failed to Approve, see if token exist and see txhash", Colors.Green);
                        }
           
                    }

                DoingSomething = false;
                AproveBtn.IsEnabled = true;
                buyBtn.IsEnabled = true;
                sellBtn.IsEnabled = true;
                sellBtnAll.IsEnabled = true;
                sellBtnX.IsEnabled = true;

            }

            private  void sellBtn_Click(object sender, RoutedEventArgs e)
            {
               
                    Sell();
                
                
            }

            public async void Sell()
            {
                decimal slip = 0;
                AproveBtn.IsEnabled = false;
                buyBtn.IsEnabled = false;
                sellBtn.IsEnabled = false;
                sellBtnAll.IsEnabled = false;
                sellBtnX.IsEnabled = false;
                DoingSomething = true;
                List<string> path = new List<string>();
                TxResult result = new TxResult();
                if (amountSell.Text != "" && gweiAmount_Copy.Text != ""&& SlipSell.Text!=""&&sellxText.Text!="")
                {
                    if (tokenBuy.Text.Length == 42)
                    {
                        if (decimal.Parse(amountSell.Text) > 0)
                        {
                            if (int.Parse(gweiAmount_Copy.Text) > 4)
                            {
                                if (int.Parse(sellxText.Text) > 0&& int.Parse(sellxText.Text) < 100)
                                {
                                    if (int.Parse(SlipSell.Text) > 3&&int.Parse(SlipSell.Text) <100)
                                    {
                                        Consola1.WriteOutput(
                                            Environment.NewLine + "Selling " + tokenName.Content + " amount: " + amountSell.Text,
                                            Colors.Green);
                                        if (fromBuy.Text == "BNB")
                                        {
                                            if (!slipMax.IsOn)
                                            {
                                   
                                                var amount = await BotHandler.SlippageTask(tokenBuy.Text,bnbcontrac,
                                                    amountSell.Text);
                                                slip = ((decimal)(Web3.Convert.FromWei(amount,
                                                    UnitConversion.EthUnit.Ether))) * ((100 - decimal.Parse(SlipSell.Text)) / 100);
                                            }

                                            {
                                                path.Add(tokenBuy.Text);
                                                path.Add(bnbcontrac);
                                                
                                            }

                                        }
                                        else
                                        {
                                            if (fromBuy.Text == "BUSD")
                                            {
                                                if (!slipMax.IsOn)
                                                {
                                        
                                                    var amount = await BotHandler.SlippageTask( tokenBuy.Text,busdcontrac,
                                                        amountSell.Text);
                                                    slip = ((decimal)(Web3.Convert.FromWei(amount,
                                                        UnitConversion.EthUnit.Ether))) * ((100 - decimal.Parse(SlipSell.Text)) / 100)*bnbPrice;
                                                }
                                            
                                                path.Add(tokenBuy.Text);
                                                path.Add(busdcontrac);
                                                path.Add(bnbcontrac);
                                            
                                    
                                            }
                                            else
                                            {
                                                if (fromBuy.Text == "USDT")
                                                {
                                                    if (!slipMax.IsOn)
                                                    {
                                                        var amount = await BotHandler.SlippageTask(tokenBuy.Text, usdtContract,
                                                            amountSell.Text);
                                                        slip = ((decimal)(Web3.Convert.FromWei(amount,
                                                            UnitConversion.EthUnit.Ether))) * ((100 - decimal.Parse(SlipSell.Text)) / 100)* bnbPrice;
                                                    }
                                                    {
                                                        path.Add(tokenBuy.Text);
                                                        path.Add(usdtContract);
                                                        path.Add(bnbcontrac);
                                                       
                                                    }
                                                }
                                            }

                                        }

                                    }
                                    else
                                    {
                                        Consola1.WriteOutput(Environment.NewLine + "Slip % incorrect, min 5 max 100", Colors.Red);
                             
                                    }
                                }
                                else
                                {
                                    Consola1.WriteOutput(Environment.NewLine + "Sell % incorrect", Colors.Red);
                           
                                }
                            }
                            else
                            {
                                Consola1.WriteOutput(Environment.NewLine + "Gas too low or incorrect", Colors.Red);
                            
                            }
                        }
                        else
                        {
                            Consola1.WriteOutput(Environment.NewLine + "Gas too low or higher than Balance", Colors.Red);
                   
                        }
                    }
                    else
                    {
                        Consola1.WriteOutput(Environment.NewLine + "Wrong address", Colors.Red);
                     
                    }
                }
                else
                {
                    Consola1.WriteOutput(Environment.NewLine + "Wrong input on gas and amount", Colors.Red);
                    
                }

            if (path.Count > 0)
            {
               
                    result = await BotHandler.DeTokenABNB(amountSell.Text, slip, path, gweiAmount_Copy.Text);
                    resultsBuy.Add(result);
                if (result.result == "Success")
                {



                    tokenPriceSell = decimal.Parse(result.ValueSpend) * tokenPriceLast;
                    tokensellPrice.Content = tokenPriceSell.ToString();
                    if (tokenPriceSell > tokenPriceBuy)
                    {
                        tokensellPrice.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        tokensellPrice.Foreground = new SolidColorBrush(Colors.Red);
                    }

              
                }
            }
            DoingSomething = false;
                AproveBtn.IsEnabled = true;
                buyBtn.IsEnabled = true;
                sellBtn.IsEnabled = true;
                sellBtnAll.IsEnabled = true;
                sellBtnX.IsEnabled = true;
              
            }
            private async void tokenBuy_TextChanged(object sender, TextChangedEventArgs e)
            {
                var text = (TextBox) sender;
                try
                {
                    if (text.Text.IsValidEthereumAddressHexFormat())
                    {
                        var name = await BotHandler.GetNameTask(text.Text);
                        await Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                            tokenName.Content = name.ToString() + " Value"; tokenNameB.Content = name.ToString() + " Balance";
                        }), DispatcherPriority.Render);
                    }
                }
                catch (Exception exception)
                {
                    
                }
           
            }

            public async Task AutosellTask(int delay, decimal profit)
            {
                var calc = tokenPriceBuy * profit;
                try
                {
                    await Task.Delay(delay);
                    if (profit > 0)
                    {
                        while (true)
                        {
                            if (tokenPriceLast >= calc)
                            {
                                if (!DoingSomething)
                                {
                                    Sell();
                                }
                                break;
                            }

                            await Task.Delay(300);
                        }

                    }
                    else
                    {
                        if (!DoingSomething)
                            Sell();

                    }
            }
                catch (Exception e)
                {
                    Consola1.WriteOutput("Failed AutoSell, use manual sell",Colors.Red);
                    
                }
                
            }
            private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                try
                {
                    if (sender.GetType() == typeof(DataGridCell))
                    {
                        var cell = (DataGridCell)sender;
                        var item = (TxResult)cell.DataContext;

                        // The cell content depends on the column used.
                        if (cell.Column.Header.ToString() == "TXHASH")
                        {
                            if (item.txHash != null)
                            {
                                ;
                                OpenUrl("https://bscscan.com/tx/" + item.txHash);

                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                
                }
           


            }
        
            private void OpenUrl(string url)
            {
                try
                {
                    Process.Start(url);
                }
                catch
                {
                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        Process.Start("open", url);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

      

            private void stop_Click(object sender, RoutedEventArgs e)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
            }

            private void Button_Click_2(object sender, RoutedEventArgs e)
            {
                Properties.Settings.Default.Wallet = walletAddress.Text;
                Properties.Settings.Default.BSCNODE = bscNode.Text;
                Properties.Settings.Default.PK = pkAddress.Password;
                Properties.Settings.Default.Save();
                ValDatos();
            }


            private void sellBtnAll_Click(object sender, RoutedEventArgs e)
            {
                amountSell.Text = tokenBalanceD.ToString();
                Sell();
            }
            private void buyTxGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
            {
                try
                {
                    if (sender.GetType() == typeof(DataGrid))
                    {
                        var data = (DataGrid)sender;
                        var cell = data.CurrentCell;
                        var item = (TxResult)cell.Item;

                        // The cell content depends on the column used.
                        if (cell.Column.Header.ToString() == "TXHASH")
                        {
                            if (item.txHash != null)
                            {
                                ;
                                OpenUrl("https://bscscan.com/tx/" + item.txHash);

                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                
                }
            
            }

            private void Button_Click_3(object sender, RoutedEventArgs e)
            {
                Clipboard.SetText(donateAddress.Text);
            }

            private void sellBtnX_Click(object sender, RoutedEventArgs e)
            {
                decimal sellamount = tokenBalanceD - (tokenBalanceD * ((100 - decimal.Parse(sellxText.Text)) / 100));
                amountSell.Text = sellamount.ToString();
                Sell();
            }

            private void tokenVl_ContentChanged(object sender, EventArgs e)
            {var lab = (CustomLabel) sender;
                decimal count;
                try
                {
                    if (decimal.Parse(lab.Content.ToString(), CultureInfo.InvariantCulture) > tokenPriceLast)
                    {
                        tokenPriceLast = decimal.Parse(lab.Content.ToString(), CultureInfo.InvariantCulture);
                        tokenVl.Foreground = new SolidColorBrush(Colors.LightSeaGreen);

                    }
                    if (decimal.Parse(lab.Content.ToString(), CultureInfo.InvariantCulture) < tokenPriceLast)
                    {
                        tokenPriceLast = decimal.Parse(lab.Content.ToString(),CultureInfo.InvariantCulture);
                        tokenVl.Foreground = new SolidColorBrush(Colors.Red);

                    }
                    if (tokenBalance.Text != "0")
                    {
                        count = tokenPriceLast * decimal.Parse(tokenBalance.Text);
                        Balance_Value.Text = count.ToString();
                    }
                    else
                    {
                        Balance_Value.Text = "0";
                    }
                }
                catch (Exception exception)
                {
                
                }

            }

            private void amountBuy_TextChanged(object sender, TextChangedEventArgs e)
            {

            }

            private void Balance_Value_TextChanged(object sender, TextChangedEventArgs e)
            {
                var lab = (TextBox)sender;
                try
                {
                    if (decimal.Parse(lab.Text) > tokenPriceLast)
                    {
                        tokenPriceAmountLast = decimal.Parse(lab.Text);
                        Balance_Value.Foreground = new SolidColorBrush(Colors.LightSeaGreen);

                    }
                    if (decimal.Parse(lab.Text) < tokenPriceLast)
                    {
                        tokenPriceAmountLast = decimal.Parse(lab.Text);
                        Balance_Value.Foreground = new SolidColorBrush(Colors.Red);

                    }
                }
                catch (Exception exception)
                {

                }

            }

            private void tokenBalance_TextChanged(object sender, TextChangedEventArgs e)
            {

            }

        private async void SwapperSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           /* var text = (ComboBox) sender;
            await Task.Delay(100);
            switch (text.Text)
            {
                case "PancakeSwap":
                    currentFactory = pancakeSwapFactoryAddress;
                    currentRouter = panacakSwapRouter;
                    break;
                case "ApeSwap":
                    currentFactory = apeSwapFactory;
                    currentRouter = apeSwapRouter;
                    break;
                case "BiSwap":
                    currentFactory = biSwapFactoy;
                    currentRouter = biSwapRouter;
                    break;
                default:
                    currentFactory = pancakeSwapFactoryAddress;
                    currentRouter = panacakSwapRouter;

                    break;
            }*/
        }
    }



  
    public class AddressValid : ValidationRule
    {
    

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (((string)value).Length == 42)
            {
               return new ValidationResult(true, null); 
            }
            else
            {

                return new ValidationResult(false,
                    "Please enter a valid address");
            }
        }
    }
    public class AmountValid : ValidationRule
    {


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var text = double.Parse((string)value);
                if (text > 0)
                {
                    return new ValidationResult(true, null);
                }
                else
                {
                    return new ValidationResult(false,
                        "Please enter a valid Amount, use , not . for decimals");
                }
            }
            catch (Exception e)
            {return new ValidationResult(false,
                "Please enter a valid Amount, use , not . for decimals");
                
            }
            
        }
    }
   
    public class GasValid : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var text = int.Parse((string)value);
                if (text>= 5 && text < 500)
                {
                    return new ValidationResult(true, null);
                }
                else
                {
                    if (text > 200)
                    {
                        return new ValidationResult(false,
                            "Too high gas, may be too expensive");
                    }
                    return new ValidationResult(false,
                        "Please enter a valid gas amount min 5 recommended 20");
                }
            }
            catch (Exception e)
            {
                
                
                return new ValidationResult(false,
                    "Please enter a valid gas amount min 5 recommended 20");
            }
            
        }
    }
    public class SellValid : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var text = int.Parse((string)value);
                if (text >= 1 && text <= 100)
                {
                    return new ValidationResult(true, null);
                }
                else
                {
                    
                    return new ValidationResult(false,
                        "Please enter a valid % sell amount min 1 recommended 100, integer only");
                }
            }
            catch (Exception e)
            {


                return new ValidationResult(false,
                    "Please enter a valid % sell amount min 1 recommended 100, integer only");
            }

        }
    }
    public class ProfitValid : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var text = decimal.Parse((string)value);
                if (text >= 1 && text <= 100)
                {
                    return new ValidationResult(true, null);
                }
                else
                {

                    return new ValidationResult(false,
                        "Please enter a valid profit amount min 1.001 Max 100x");
                }
            }
            catch (Exception e)
            {


                return new ValidationResult(false,
                    "Please enter a valid profit amount min 1.001 Max 100x");
            }

        }
    }
    public class SlipValid : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var text = int.Parse((string)value);
                if (text >= 5 && text < 500)
                {
                    return new ValidationResult(true, null);
                }
                else
                {
                    if (text > 200)
                    {
                        return new ValidationResult(false,
                            "Too high slip, may as well use max slip");
                    }
                    return new ValidationResult(false,
                        "Please enter a valid % slip amount min 5 recommended 40");
                }
            }
            catch (Exception e)
            {


                return new ValidationResult(false,
                    "Please enter a valid % slip amount min 5 recommended 40");
            }

        }
    }
    public class TxResult
    {
        public string txHash { get; set; }
        public string result { get; set; }
        public string value { get; set; }
        public string ValueSpend { get; set; }
        public DateTime Time { get; set; }

    }
    public class MyDataSource
    {
        public MyDataSource()
        {
            address = "0x";
            address2 = "0x";
            wallet = "0x";
            amount1 = "0";
            amount2 = "0";
            gas1 = "25";
            gas2 = "25";
            sell = "0";
            Slip = "50";
            Slip2 = "50";
            Profit = "0";
            delay = "0";
            sell2 = "10";
        }

        public string address { get; set; }
        public string address2 { get; set; }
        public string wallet { get; set; }
        public string amount1 { get; set; }
        public string amount2 { get; set; }
        public string gas1 { get; set; }
        public string gas2 { get; set; }
        public string sell { get; set; }
        public string Slip { get; set; }
        public string sell2 { get; set; }
        public string Profit { get; set; }
        public string Slip2 { get; set; }
        public string delay { get; set; }
    }
    

}
