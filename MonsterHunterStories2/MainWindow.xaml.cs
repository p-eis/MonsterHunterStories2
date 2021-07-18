﻿using System;
using System.Windows;
using Microsoft.Win32;

namespace MonsterHunterStories2
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_PreviewDragOver(object sender, DragEventArgs e)
		{
			e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
		}

		private void Window_Drop(object sender, DragEventArgs e)
		{
			String[] files = e.Data.GetData(DataFormats.FileDrop) as String[];
			if (files == null) return;

			SaveData.Instance().Open(files[0]);
			DataContext = new ViewModel();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Properties.Settings.Default.Save();
		}

		private void MenuItemFileOpen_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new OpenFileDialog();
			if (dlg.ShowDialog() == false) return;

			SaveData.Instance().Open(dlg.FileName);
			DataContext = new ViewModel();
		}

		private void MenuItemFileSave_Click(object sender, RoutedEventArgs e)
		{
			SaveData.Instance().Save();
		}

		private void MenuItemFileSaveAs_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new SaveFileDialog();
			if (dlg.ShowDialog() == false) return;

			SaveData.Instance().SaveAs(dlg.FileName);
		}

		private void MenuItemFileExit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new AboutWindow();
			dlg.ShowDialog();
		}

		private void ButtonChoiceItem_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new ChoiceWindow();
			dlg.ID = 0;
			dlg.ShowDialog();
			if (dlg.ID == 0) return;

			uint id = dlg.ID;
			ViewModel viewmodel = DataContext as ViewModel;
			if (viewmodel == null) return;

			// 重複チェック
			for(int i = 0; i < viewmodel.Items.Count; i++)
			{
				if(viewmodel.Items[i].ID == id)
				{
					ListBoxItem.SelectedIndex = i;
					return;
				}
			}

			// アイテム追加
			Item item = new Item(Util.ItemIDAddress(id));
			item.ID = id;
			item.Count = 1;
			viewmodel.Items.Add(item);

			// アイテム持ちのフラグ設定
			SaveData.Instance().WriteBit(0x12B68 + id / 8, id % 8, true);
		}

		private void ButtonChoiceMonster_Click(object sender, RoutedEventArgs e)
		{
			Monster monster = ListBoxMonster.SelectedItem as Monster;
			if (monster == null) return;

			var dlg = new ChoiceWindow();
			dlg.ID = monster.ID;
			dlg.Type = ChoiceWindow.eType.TYPE_MONSTER;
			dlg.ShowDialog();
			monster.ID = dlg.ID;
		}

		private void ButtonChoiceGenes_Click(object sender, RoutedEventArgs e)
		{
			Gene gene = ListBoxGenes.SelectedItem as Gene;
			if (gene == null) return;

			var dlg = new ChoiceWindow();
			dlg.ID = gene.ID;
			dlg.Type = ChoiceWindow.eType.TYPE_GENE;
			dlg.ShowDialog();
			gene.ID = dlg.ID;
		}

		private void ButtonChoiceRaidAction1_Click(object sender, RoutedEventArgs e)
		{
			Monster monster = ListBoxMonster.SelectedItem as Monster;
			if (monster == null) return;
			monster.RideAction1 = ChoiceMonsterRideAction(monster.RideAction1);
		}

		private void ButtonChoiceRaidAction2_Click(object sender, RoutedEventArgs e)
		{
			Monster monster = ListBoxMonster.SelectedItem as Monster;
			if (monster == null) return;
			monster.RideAction2 = ChoiceMonsterRideAction(monster.RideAction2);
		}

		private void ButtonAppendEgg_Click(object sender, RoutedEventArgs e)
		{
			uint count = (uint)ListBoxEgg.Items.Count;
			if (count >= Util.EGG_COUNT) return;

			ViewModel viewmodel = DataContext as ViewModel;
			if (viewmodel == null) return;
			Egg egg = new Egg(Util.EGG_ADDRESS + count * Util.EGG_SIZE);
			viewmodel.Eggs.Add(egg);
			SaveData.Instance().WriteNumber(Util.EGG_COUNT_ADDRESS, 1, count);
		}

		private uint ChoiceMonsterRideAction(uint id)
		{
			var dlg = new ChoiceWindow();
			dlg.ID = id;
			dlg.Type = ChoiceWindow.eType.TYPE_RAIDACTION;
			dlg.ShowDialog();
			return dlg.ID;
		}

		private void MaxoutStack_Click(object sender, RoutedEventArgs e)
		{
			Monster monster = ListBoxMonster.SelectedItem as Monster;
			if (monster == null) return;

			monster.MaximizeGeneStack();
		}
	}
}
