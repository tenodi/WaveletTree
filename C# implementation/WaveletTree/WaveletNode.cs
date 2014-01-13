using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wavelet
{
	/*	 This class implements a node in a Wavelet tree*/
	class WaveletNode 
	{
		private String bitmap;

		private WaveletNode parent;
		private WaveletNode leftChild;
		private WaveletNode rightChild;

		//Get bitmap  
		public String getBitmap()
		{
			return bitmap;
		}

		//Set bitmap
		public void setBitmap(String bitmap)
		{
			this.bitmap = bitmap;
		}

		//Get left child
		public WaveletNode getLeftChild()
		{
			return leftChild;
		}

		//Get right child
		public WaveletNode getRightChild()
		{
			return rightChild;
		}

		//Set left child
		public void setLeftChild(WaveletNode child)
		{
			this.leftChild = child;
		}

		//Set right child
		public void setRightChild(WaveletNode child)
		{
			this.rightChild = child;
		}

		//Set parent
		public void setParent(WaveletNode parent)
		{
			this.parent = parent;
		}

		//Get parent
		public WaveletNode getParent()
		{
			return this.parent;
		}
	}
}
