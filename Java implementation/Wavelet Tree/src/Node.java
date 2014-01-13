// represents node in wavelet tree
public class Node {

	// bits for characters represented by node
	private String bitmap;
	
	// references
	private Node parent;
	private Node leftChild;
	private Node rightChild;
	
	
	// getters and setters for member variables
	public String getBitmap()
	{
		return bitmap;
	}
	
	public void setBitmap (String bitmap)
	{
		this.bitmap = bitmap;
	}
	
	public Node getLeftChild()
	{
		return leftChild;
	}
	
	public Node getRightChild()
	{
		return rightChild;
	}
	
	public void setLeftChild(Node child)
	{
		this.leftChild = child;
	}

	public void setRightChild(Node child)
	{
		this.rightChild = child;
	}
	
	public void setParent(Node parent)
	{
		this.parent = parent;
	}
	
	public Node getParent()
	{
		return this.parent;
	}
}
