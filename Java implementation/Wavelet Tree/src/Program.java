
public class Program {

	public static void main(String args[])
	{
		Runtime.getRuntime().gc();
		
		System.out.println("Java implementation starting...\n");
		System.out.println("Total memory taken by JVM before starting is " + getMb(Runtime.getRuntime().totalMemory()) + "MB\n");
		
		
		System.out.println("\nStarting program:");
		System.out.println("-------------");
		
		try
		{
			new WaveletTreeConstructor(args);
		}
		catch (Exception e)
		{
			System.out.println(e.getMessage());
		}
	}
	
	public static long getKb(long bytes){
		return bytes / (1024);
	}
	
	public static long getMb(long bytes){
		return bytes / (1024 * 1024);
	}
	
}
