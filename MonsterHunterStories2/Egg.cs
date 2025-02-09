﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MonsterHunterStories2
{
	class Egg : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public ObservableCollection<Gene> Genes { get; set; } = new ObservableCollection<Gene>();

		private readonly uint mAddress;

		public Egg(uint address)
		{
			mAddress = address;
			for (uint i = 0; i < 9; i++)
			{
				Genes.Add(new Gene(address + 8 + 4 * i));
			}
		}

		public uint ID
		{
			get { return SaveData.Instance().ReadNumber(mAddress, 4); }
			set
			{
				SaveData.Instance().WriteNumber(mAddress, 4, value);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ID)));
			}
		}

		public uint Smell
		{
			get { return SaveData.Instance().ReadNumber(mAddress + 4, 1); }
			set
			{
				Util.WriteNumber(mAddress + 4, 1, value, 0, 2);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Smell)));
			}
		}
	}
}
