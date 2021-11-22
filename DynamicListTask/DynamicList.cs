using System;
using System.Collections;
using System.Collections.Generic;

namespace DynamicListTask
{
	public class DynamicList<T> : IEnumerable<T>
	{
		private const int MaxArrayLength = 0X7FEFFFFF;

		private const int defaultCapacity = 8;
		private T[] data;
		//Number of elements that data array contains.
		private int count;
		private int verion;

		public DynamicList()
			:this(defaultCapacity)
		{
		}

		public DynamicList(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentException(nameof(capacity), $"{nameof(capacity)} can not be less than 0");
			}

			this.data = new T[capacity];
			this.count = 0;
			this.verion = 0;
		}

		public DynamicList(IEnumerable<T> collection)
			:this(defaultCapacity)
		{
			if (collection == null)
			{
				throw new ArgumentNullException(nameof(collection));
			}

			foreach (var element in collection)
			{
				if (this.count == this.data.Length)
				{
					this.Resize(count + 1);
				}

				this.data[this.count] = element;
				this.count++;
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		private int Capacity
		{
			get
			{
				return this.data.Length;
			}

			set
			{
				if (value < this.count)
				{
					throw new ArgumentOutOfRangeException($"{nameof(Capacity)} can not be less than number of elemnts in list.");
				}

				if (value != this.data.Length)
				{
					var newArray = new T[value];
					Array.Copy(this.data, newArray, count);
					this.data = newArray;
				}
			}
		}

		public void Add(T value)
		{
			if (this.count == this.data.Length)
			{
				this.Resize(count + 1);
			}

			this.data[count++] = value;
			this.verion++;
		}

		public bool Remove(T value)
		{
			int index = this.IndexOf(value);
			if (index >= 0)
			{
				this.RemoveAt(index);
				return true;
			}

			return false;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.count)
			{
				throw new IndexOutOfRangeException($"{nameof(index)} is less than 0 or more than ${this.Count}");
			}

			Array.Copy(this.data, index + 1, this.data, index, this.count - index - 1);
			this.count--;
			this.verion++;
		}

		public int IndexOf(T value)
		{
			var eComparer = EqualityComparer<T>.Default;
			for (int i = 0; i < this.count; i++)
			{
				if (eComparer.Equals(this.data[i], value))
				{
					return i;
				}
			}

			return -1;
		}

		public void Cleare()
		{
			if (this.count > 0)
			{
				this.count = 0;
			}

			verion++;
		}

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= this.count)
				{
					throw new IndexOutOfRangeException($"{nameof(index)} is less than 0 or more than {nameof(this.Count)}");
				}

				return this.data[index];
			}

			set
			{
				if (index < 0 || index >= this.count)
				{
					throw new IndexOutOfRangeException($"{nameof(index)} is less than 0 or more than {nameof(this.Count)}");
				}

				this.data[index] = value;
			}
		}

		private void Resize(int min)
		{
			if (this.data.Length < min)
			{
				int newCapacity = this.data.Length == 0 ? defaultCapacity : this.data.Length * 2;
				if ((uint)newCapacity > MaxArrayLength)
				{
					newCapacity = MaxArrayLength;
				}

				if (newCapacity < min)
				{
					newCapacity = min;
				}

				Capacity = newCapacity;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new DynamicListEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new DynamicListEnumerator(this);
		}

		public struct DynamicListEnumerator : IEnumerator<T>
		{
			private DynamicList<T> dynamicList;
			private int index;
			private int verison;

			public DynamicListEnumerator(DynamicList<T> list)
			{
				this.dynamicList = list;
				this.index = -1;
				this.verison = list.verion;
			}

			public T Current
			{
				get
				{
					if ((uint)index < 0 || (uint)index >= (uint)this.dynamicList.count)
                    {
						return default(T);
					}

					return this.dynamicList.data[this.index];
				}
			}

			object IEnumerator.Current => this.Current;

			public void Dispose() {}

			public bool MoveNext()
			{
				if (this.verison != this.dynamicList.verion)
                {
					throw new InvalidOperationException($"{nameof(DynamicList<T>)} can not be changed during iteration");
                }

				return ++index < this.dynamicList.count; 
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}
		}

	}
}
