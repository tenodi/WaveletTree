using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace Wavelet
{
	/*	This class implements Wavelet tree structure*/
	class WaveletTree
	{

		private String sequence;
		private String commentLines;	

		private ArrayList alphabet;
		private WaveletNode root;

		private char character;
		private int position;
		private char characterSelect;
		private int positionMember;
		private int occurrence;

		// Invoking main methods (rank, select, member)
		public WaveletTree(string[] args)
		{
			alphabet = new ArrayList();
			root = new WaveletNode();

			if (args.Length != 6)
			{
				throw new Exception("Wrong number of arguments.");
			}

			Stopwatch stopWatch1 = new Stopwatch();
			stopWatch1.Start();
			readFile(args[0]);
			stopWatch1.Stop();
			Console.WriteLine("Identity : " + commentLines);
			Console.WriteLine("Fasta file has " + sequence.Length + " characters");
			Console.WriteLine("Reading fasta file took: " + stopWatch1.Elapsed.TotalMilliseconds + " ms");


			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			determineAlphabet();
			stopWatch.Stop();
			Console.WriteLine("Determination of alphabet took: " + stopWatch.Elapsed.TotalMilliseconds+ " ms");

			Stopwatch stopWatch2 = new Stopwatch();
			stopWatch2.Start();
			generateWaveletTree(alphabet, sequence, root);
			stopWatch2.Stop();
			Console.WriteLine("Generating tree took: " + stopWatch2.Elapsed.TotalMilliseconds + " ms");


			// initialisation for SELECT and MEMEBER and RANK

			try
			{

				character =Char.ToUpper( Char.Parse(args[1]));
				position = int.Parse(args[2]);
				occurrence = int.Parse(args[3]);
				characterSelect = Char.ToUpper(char.Parse(args[4]));
				positionMember = int.Parse(args[5]);

			}catch(Exception e){
				throw new Exception("Arguments are not correct.");
			}

			Console.WriteLine("\nTesting functions on tree:\n");

			Console.WriteLine("\nRank:");
			Stopwatch stopWatch3 = new Stopwatch();
			stopWatch3.Start();
			Console.WriteLine("Number of occurrences of character " + character + " till position " + position +
				" is " + rank(root, position - 1, character, alphabet));
			stopWatch3.Stop();

			Console.WriteLine("Function RANK took: " + stopWatch3.Elapsed.TotalMilliseconds + " ms");




			Console.WriteLine("\nSelect:");
			Stopwatch stopWatch4 = new Stopwatch();
			stopWatch4.Start();
			Console.WriteLine("Position of " + occurrence + ". occurrence of character " +
				characterSelect + " is " + (select(occurrence, characterSelect)));
			stopWatch4.Stop();
			Console.WriteLine("Function SELECT took: " + stopWatch4.Elapsed.TotalMilliseconds + "ms");



			Console.WriteLine("\nMember:");
			Stopwatch stopWatch5 = new Stopwatch();
			stopWatch5.Start();
			Console.WriteLine("Character at position " + (positionMember) + " is " + member(positionMember - 1));
			stopWatch5.Stop();
			Console.WriteLine("Function MEMBER took: " + stopWatch5.Elapsed.TotalMilliseconds +  "ms");



		}


		// constructing functions:

		// Reads fasta file splitting comments from data
		private void readFile(String fastaFilePath)
		{

			try
			{
				string[] lines= System.IO.File.ReadAllLines(fastaFilePath);

				StringBuilder sb = new StringBuilder();
				StringBuilder comments = new StringBuilder();

				foreach (string line in lines)
				{
					if (!(line.StartsWith(">") || line.StartsWith(";")))
					{
						sb.Append( line);
					}
					else
					{
						comments.Append(line); 
					}

					sequence = sb.ToString();
					commentLines = comments.ToString();           
				}
			}
			catch (Exception e)
			{
				throw new Exception("Wrong file path.");
			}

		}


		// Determining alphabet from characters contained in fasta format file
		public void determineAlphabet()
		{
			if (sequence.Length > 0)
			{


				foreach (char c in sequence)
				{
					if (!alphabet.Contains(Char.ToUpper(c)))
					{
						alphabet.Add(Char.ToUpper(c));

					}
				}

				alphabet.Sort();


			}
			else
			{
				throw new Exception("Empty file.");
			}
		}

		// Generating Wavelet tree from fasta format
		public void generateWaveletTree(ArrayList currentAlphabet, String currentLabel, WaveletNode currentNode)
		{
			if (currentAlphabet.Count > 2)
			{
				StringBuilder bitmapBuilder = new StringBuilder();
				StringBuilder leftLabel = new StringBuilder();
				StringBuilder rightLabel = new StringBuilder();
				int middle = (currentAlphabet.Count + 1) / 2; 

				foreach (char c in currentLabel)
				{
					if (getIndex(c, currentAlphabet) < middle) 
					{
						bitmapBuilder.Append("0");
						leftLabel.Append(Char.ToUpper(c));
					}
					else
					{
						bitmapBuilder.Append("1");
						rightLabel.Append(Char.ToUpper(c));
					}
				}


				currentNode.setBitmap(bitmapBuilder.ToString());

				// size of alphabet is certainly greater than 2, so left child of current node will exist for sure,
				// because already for 3, we'll have alphabet of 2 in left child.
				// right child will exist only if alphabet is greater than 3, and only then we'll set that object
				// and call recursion


				// left side
				currentNode.setLeftChild(new WaveletNode());
				currentNode.getLeftChild().setParent(currentNode);

				//var x = currentAlphabet.GetRange(0, (currentAlphabet.Count - (middle - 1)));
				generateWaveletTree(currentAlphabet.GetRange(0, middle), leftLabel.ToString(), currentNode.getLeftChild());

				// right side
				if (currentAlphabet.Count > 3)
				{
					currentNode.setRightChild(new WaveletNode());
					currentNode.getRightChild().setParent(currentNode);
					generateWaveletTree(currentAlphabet.GetRange(middle, (currentAlphabet.Count-middle)), 
						rightLabel.ToString(), currentNode.getRightChild());
				}


			}
			else
			{
				// we still need to set the bitmap of that node, if the alphabet is 2!
				if (currentAlphabet.Count == 2)
				{
					StringBuilder bitmapBuilder = new StringBuilder();

					foreach (char c in currentLabel)
					{
						if (getIndex(c, currentAlphabet) + 1 == 1) // we're hardcoding breakpoint
						{
							bitmapBuilder.Append("0");
						}
						else
						{
							bitmapBuilder.Append("1");
						}
					}

					currentNode.setBitmap(bitmapBuilder.ToString());
				}

				return;
			}

		}






		// Returns number of occurrences till some position of one character
		public int rank(WaveletNode currentNode, int index, char character, ArrayList currentAlphabet){

			if (!currentAlphabet.Contains(character)){
				return 0;			
			}

			int middle = (currentAlphabet.Count + 1) / 2;
			int newIndex;
			ArrayList currentAlphabetSliced = new ArrayList();

			if (getIndex(character, currentAlphabet) < middle) // if character is represented by 0
			{
				newIndex = index - rank1(currentNode.getBitmap(), index);
				currentNode = currentNode.getLeftChild();
				currentAlphabetSliced = currentAlphabet.GetRange(0, (currentAlphabet.Count - (middle - 1)));

			}
			else
			{
				newIndex = rank1(currentNode.getBitmap(), index) - 1;
				currentNode = currentNode.getRightChild();
				currentAlphabetSliced = currentAlphabet.GetRange(middle, (currentAlphabet.Count - middle));
			}

			if (currentNode != null)
			{
				return rank(currentNode, newIndex, character, currentAlphabetSliced);
			}
			else
			{
				return newIndex + 1;
			}
		}


		// Returns position of nth occurrence of character
		public int select (int nthOccurrence, char character)
		{

			Interval alphabeticInterval = new Interval(0, alphabet.Count - 1);
			WaveletNode currentNode = root;
			int indexOfCharInAlph = getIndex(character, alphabet);
			bool characterRepresentedWithZero = true;

			while (alphabeticInterval.isGreaterThanTwo()) 
			{
				if (alphabeticInterval.getSize() == 3)
				{
					if (alphabeticInterval.getRightIndex() == indexOfCharInAlph) 

					{
						characterRepresentedWithZero = false;
						break;
					}
				}

				if (indexOfCharInAlph <= alphabeticInterval.getMiddleIndex())
				{
					currentNode = currentNode.getLeftChild();
					alphabeticInterval.setRightIndex();
				}
				else
				{
					currentNode = currentNode.getRightChild();
					alphabeticInterval.setLeftIndex();
				}
			}

			if (characterRepresentedWithZero) 
			{
				if (alphabeticInterval.getLeftIndex() == indexOfCharInAlph)
					characterRepresentedWithZero = true;
				else
					characterRepresentedWithZero = false;
			}



			// tree traversal bottom-up once we have node representing our character
			int position = getPositionOfNthOccurrence(currentNode.getBitmap(), nthOccurrence, characterRepresentedWithZero); 
			if (position == 0)
			{
				return -1; // that indicates that there is no n occurrences of character!
			}

			WaveletNode child = currentNode;
			currentNode = currentNode.getParent();

			while(currentNode != null)
			{
				if (currentNode.getLeftChild().Equals(child))
				{
					position = getPositionOfNthOccurrence(currentNode.getBitmap(), position, true);
				}
				else
				{
					position = getPositionOfNthOccurrence(currentNode.getBitmap(), position, false);
				}

				currentNode = currentNode.getParent();
				child = child.getParent();
			}

			return position;
		}


		// Returns character at index
		public char member (int index) 
		{
			Interval alphabeticInterval = new Interval (0, alphabet.Count - 1);

			if (root.getBitmap().Length - 1 < index)
			{
				throw new Exception("Index out of range for function memeber.");
			}
			else
			{
				WaveletNode currentNode = root;
				int newIndex = index;

				while (alphabeticInterval.isGreaterThanTwo())
				{

					if (currentNode != null) // currentNode can be set to null because we're NOT storing leafs which would have only 
						// 1 character and therefore, we need to store parent also.
					{
						index = newIndex;

						if (currentNode.getBitmap().ElementAt(index) == '1')
						{

							newIndex = rank1(currentNode.getBitmap(), index) - 1;
							currentNode = currentNode.getRightChild();
							alphabeticInterval.setLeftIndex();
						}
						else
						{
							newIndex = index - rank1(currentNode.getBitmap(), index); // we're counting 0s!
							currentNode = currentNode.getLeftChild();
							alphabeticInterval.setRightIndex();
						}
					}
					else
					{
						break;
					}
				}

				if (currentNode != null)
				{
					// now that we're done, we need to check if it's 1 or 0
					if (currentNode.getBitmap().ElementAt(newIndex) == '1') // we're taking right interval!
					{

						return (char)alphabet[alphabeticInterval.getRightIndex()];
					}
					else
					{
						return (char)alphabet[alphabeticInterval.getLeftIndex()];
					}
				}
				else // it doesn't matter which index we use here, because both are the same
				{
					return (char)alphabet[alphabeticInterval.getLeftIndex()];
				}

			}

		}







		// Returns position of nth occurrence in bitmap of 0 of 1, depending on boolean parameter
		public int getPositionOfNthOccurrence(String bitmap, int nthOcurrance, bool characterRepresentedWithZero)
		{
			int counter = 0;
			int position = 0; 
			foreach (char c in bitmap)
			{
				if (counter < nthOcurrance)
				{
					position++;

					if (characterRepresentedWithZero && c == '0')
						counter++;
					else if (!characterRepresentedWithZero && c=='1')
						counter++;
				}
				else
					break;
			}

			if (counter == nthOcurrance) // if this is not true, that means that there are no n occurances in bitmap!
				return position;
			else
				return 0;
		}

		// Returns index of character in array of characters
		public int getIndex(char c, ArrayList arrayList)
		{
			for (int i = 0; i < arrayList.Count; i++)
			{
				if (c == (char)arrayList[i])
				{
					return i;
				}
			} 

			return -1;
		}


		// Returns numbers of 1s till the ith position, including possible 1 on the ith position
		public int rank1(String bitmap, int index)
		{
			int counter = 0;

			for (int i=0; i < bitmap.Length && i <= index; i++)
			{
				if (bitmap.ElementAt(i) == '1')
				{
					counter++;
				}
			}

			return counter;
		}





	}
}
