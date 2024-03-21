﻿using LinePutScript;
using LinePutScript.Localization.WPF;
using Panuon.WPF;
using Panuon.WPF.UI;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VPet_Simulator.Core;
using VPet_Simulator.Windows.Interface;
using static System.Windows.Forms.LinkLabel;
using static VPet_Simulator.Core.GraphCore;
using static VPet_Simulator.Core.GraphInfo;
using static VPet_Simulator.Core.IGraph;
using static VPet_Simulator.Windows.Interface.MPMessage;

namespace VPet_Simulator.Windows
{
    /// <summary>
    /// winMPBetterBuy.xaml 的交互逻辑
    /// </summary>
    public partial class winMPBetterBuy : WindowX
    {
        private TextBox _searchTextBox;
        MPFriends mf;
        private bool AllowChange = false;
        private Switch _puswitch;
        private int _columns;
        private int _rows;

        public winMPBetterBuy(MPFriends mf)
        {
            InitializeComponent();
            this.mf = mf;
            Title = "给更好的{0}买".Translate(mf.friend.Name);
            LsbSortRule.SelectedIndex = mf.mw.Set["betterbuy"].GetInt("lastorder");
            LsbSortAsc.SelectedIndex = mf.mw.Set["betterbuy"].GetBool("lastasc") ? 0 : 1;
            AllowChange = true;
        }
        Run rMoney;
        public void Show(Food.FoodType type)
        {
            if (!AllowChange)
                return;
            if (_searchTextBox != null)
                _searchTextBox.Text = "";
            if (LsbCategory.SelectedIndex == (int)type)
                OrderItemSource(type, LsbSortRule.SelectedIndex, LsbSortAsc.SelectedIndex == 0);
            else
                LsbCategory.SelectedIndex = (int)type;
            if (rMoney != null)
                rMoney.Text = mf.Core.Save.Money.ToString("f2");

            Show();
        }
        public void OrderItemSource(Food.FoodType type, int sortrule, bool sortasc, string searchtext = null)
        {
            Task.Run(() =>
            {
                List<Food> foods;
                switch (type)
                {
                    case Food.FoodType.Food:
                        foods = mf.Foods;
                        break;
                    case Food.FoodType.Star:
                        //List<Food> lf = new List<Food>();
                        //foreach (var sub in mf.Set["betterbuy"].FindAll("star"))
                        //{
                        //    var str = sub.Info;
                        //    var food = mf.Foods.FirstOrDefault(x => x.Name == str);
                        //    if (food != null)
                        //        lf.Add(food);
                        //}
                        //foods = lf;
                        foods = mf.Foods.FindAll(x => x.Star);
                        break;
                    default:
                        foods = mf.Foods.FindAll(x => x.Type == type);// || x.Type == Food.FoodType.Limit);
                        break;
                }
                if (!string.IsNullOrEmpty(searchtext))
                {
                    foods = foods.FindAll(x => x.TranslateName.Contains(searchtext));
                }
                IOrderedEnumerable<Food> ordered;
                switch (sortrule)
                {
                    default:
                    case 0:
                        if (sortasc)
                            ordered = foods.OrderBy(x => x.TranslateName);
                        else
                            ordered = foods.OrderByDescending(x => x.TranslateName);
                        break;
                    case 1:
                        if (sortasc)
                            ordered = foods.OrderBy(x => x.Price);
                        else
                            ordered = foods.OrderByDescending(x => x.Price);
                        break;
                    case 2:
                        if (sortasc)
                            ordered = foods.OrderBy(x => x.StrengthFood);
                        else
                            ordered = foods.OrderByDescending(x => x.StrengthFood);
                        break;
                    case 3:
                        if (sortasc)
                            ordered = foods.OrderBy(x => x.StrengthDrink);
                        else
                            ordered = foods.OrderByDescending(x => x.StrengthDrink);
                        break;
                    case 4:
                        if (sortasc)
                            ordered = foods.OrderBy(x => x.Strength);
                        else
                            ordered = foods.OrderByDescending(x => x.Strength);
                        break;
                    case 5:
                        if (sortasc)
                            ordered = foods.OrderBy(x => x.Feeling);
                        else
                            ordered = foods.OrderByDescending(x => x.Feeling);
                        break;
                    case 6:
                        if (sortasc)
                            ordered = foods.OrderBy(x => x.Health);
                        else
                            ordered = foods.OrderByDescending(x => x.Health);
                        break;
                    case 7:
                        if (sortasc)
                            ordered = foods.OrderBy(x => x.Exp);
                        else
                            ordered = foods.OrderByDescending(x => x.Exp);
                        break;
                    case 8:
                        if (sortasc)
                            ordered = foods.OrderBy(x => x.Likability);
                        else
                            ordered = foods.OrderByDescending(x => x.Likability);
                        break;
                }
                Dispatcher.Invoke(() =>
                {
                    var totalCount = ordered.Count();
                    var pageSize = _rows * _columns;
                    pagination.MaxPage = (int)Math.Ceiling(totalCount * 1.0 / pageSize);
                    var currentPage = Math.Max(0, Math.Min(pagination.MaxPage, pagination.CurrentPage) - 1);
                    pagination.CurrentPage = currentPage + 1;
                    IcCommodity.ItemsSource = ordered.Skip(pageSize * currentPage).Take(pageSize);
                });
            });
        }

        //private void RbtnIncrease_Click(object sender, RoutedEventArgs e)
        //{
        //    var repeatButton = sender as RepeatButton;
        //    var item = repeatButton.DataContext as BetterBuyItem;
        //    item.Quantity = Math.Max(1, item.Quantity + 1);
        //}

        //private void RbtnDecrease_Click(object sender, RoutedEventArgs e)
        //{
        //    var repeatButton = sender as RepeatButton;
        //    var item = repeatButton.DataContext as BetterBuyItem;
        //    item.Quantity = Math.Max(1, item.Quantity - 1);
        //}
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            //eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            //eventArg.Source = sender;
            //PageDetail.RaiseEvent(eventArg);
        }
        private void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            var Button = sender as Button;
            var item = Button.DataContext as Food;
            //看是什么模式
            bool EnableFunction = mf.mw.Set.EnableFunction && mf.mw.HashCheck && !item.IsOverLoad()
                && item.Price >= 1 && item.Price <= 1000 && item.Health >= 0 && item.Exp >= 0 &&
                item.Likability >= 0 && item.Price + 1000 < mf.mw.GameSavesData.GameSave.Money;
            //不吃负面/太贵/太便宜

            if (EnableFunction)//扣钱
                mf.mw.GameSavesData.GameSave.Money -= item.Price;

            mf.DisplayFoodAnimation(item.GetGraph(), item.ImageSource);

            if (EnableFunction)
                mf.Main.LabelDisplayShow("{0}花费${3}\n给{1}买了{2}".Translate(SteamClient.Name, mf.Core.Save.Name, item.TranslateName, item.Price));
            else
                mf.Main.LabelDisplayShow("{0}给{1}买了{2}".Translate(SteamClient.Name, mf.Core.Save.Name, item.TranslateName));

            var msg = new MPMessage()
            {
                To = mf.friend.Id.Value,
                Type = (int)MPMessage.MSGType.Feed,
            };
            var feed = new Feed()
            {
                EnableFunction = EnableFunction,
                Item = item,
            };
            msg.SetContent(feed);
            mf.wmp.SendMessageALL(msg);

            if (!_puswitch.IsChecked.Value)
            {
                Close();
            }
            else
            {
                rMoney.Text = mf.mw.Core.Save.Money.ToString("f2");
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void BtnTitle_Click(object sender, RoutedEventArgs e)
        {
            _searchTextBox.Text = "";
            Search();
        }

        private void Search()
        {
            if (!AllowChange)
                return;
            var searchText = "";
            if (_searchTextBox != null)
            {
                searchText = _searchTextBox.Text;
            }
            OrderItemSource((Food.FoodType)LsbCategory.SelectedIndex, LsbSortRule.SelectedIndex, LsbSortAsc.SelectedIndex == 0, searchText);
        }

        private void TbTitleSearch_Loaded(object sender, RoutedEventArgs e)
        {
            _searchTextBox = sender as TextBox;
        }

        private void LsbSortRule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!AllowChange)
                return;
            int order = LsbSortRule.SelectedIndex;
            bool asc = LsbSortAsc.SelectedIndex == 0;
            OrderItemSource((Food.FoodType)LsbCategory.SelectedIndex, order, asc, _searchTextBox?.Text);
        }

        private void WindowX_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mf.winMPBetterBuy = null;
        }

        private void Switch_Loaded(object sender, RoutedEventArgs e)
        {
            _puswitch = sender as Switch;
            _puswitch.IsChecked = mf.mw.Set["betterbuy"].GetBool("noautoclose");
            _puswitch.Click += Switch_Checked;
        }

        private void Switch_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void AutoUniformGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var rows = Math.Max(0, (int)Math.Floor(IcCommodity.ActualHeight / 150d));
            if (rows != _rows)
            {
                _rows = rows;
                Search();
            }
            _rows = rows;
        }

        private void AutoUniformGrid_Changed(object sender, RoutedEventArgs e)
        {
            var uniformGrid = e.OriginalSource as AutoUniformGrid;
            var columns = uniformGrid.Columns;
            if (columns != _columns)
            {
                _columns = columns;
                Search();
            }
            _columns = columns;
        }

        private void pagination_CurrentPageChanged(object sender, SelectedValueChangedRoutedEventArgs<int> e)
        {
            if (!AllowChange)
                return;
            Search();
            TbPage.Text = e.NewValue.ToString();
        }

        private void rMoney_Loaded(object sender, RoutedEventArgs e)
        {
            rMoney = sender as Run;
            rMoney.Text = mf.mw.Core.Save.Money.ToString("f2");
        }
        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Content = "给更好的{0}买".Translate(mf.friend.Name);
        }

        private void TbPage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter
                && int.TryParse(TbPage.Text?.Trim(), out int page))
            {
                pagination.CurrentPage = Math.Max(0, Math.Min(pagination.MaxPage, page));
            }
        }
    }
}
