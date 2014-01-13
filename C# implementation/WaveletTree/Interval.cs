using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wavelet
{
	/*	 This class handles alphabet indices as an interval while traversing child nodes */
	class Interval
	{
		private int leftIndex;
		private int rightIndex ;

		//Sets starting and ending index
		public Interval(int leftIndex, int rightIndex)
		{
			this.leftIndex = leftIndex;
			this.rightIndex = rightIndex;
		}

		//Get left index
		public int getLeftIndex()
		{
			return leftIndex;
		}

		//Get right index
		public int getRightIndex()
		{
			return rightIndex;
		}

		//Set left index while going into right node
		public void setLeftIndex() 
		{
			this.leftIndex = rightIndex - (getSize() / 2 - 1);
		}

		//Set right index while going into left node
		public void setRightIndex()
		{
			this.rightIndex = leftIndex + ((getSize() + 1) / 2 - 1); // getSize + 1 because on odd size of alphabet, left half is bigger for 1
		}

		//Check if interval is bigger than 2
		public bool isGreaterThanTwo()
		{
			if ((rightIndex - leftIndex) > 1) // because it's inclusive
			{
				return true;
			}
			else
			{
				return false;
			}

		}


		// Get index in the middle of the interval
		public int getMiddleIndex()
		{
			return (this.leftIndex + this.rightIndex) / 2;
		}

		//Get interval size
		public int getSize() 
		{
			return rightIndex - leftIndex + 1;
		}
	}
}
