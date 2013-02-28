import static org.junit.Assert.*;

import java.util.ArrayList;

import org.junit.Test;


public class LinkedListTest 
{

	@Test
	public void ConstructorTest() 
	{
		LinkedList<Integer> list = new LinkedList<Integer>();
		assertTrue(list.size() == 0);
	}
	
	@Test
	public void AddTest()
	{
		LinkedList<Integer> list = new LinkedList<Integer>();
		list.add(new Integer(0));
		assertTrue(list.size() == 1);
		assertTrue(list.get(0) == 0);
		list.add(new Integer(1));
		assertTrue(list.size() == 2);
		assertTrue(list.get(1) == 1);
		list.add(new Integer(2));
		assertTrue(list.size() == 3);
		assertTrue(list.get(2) == 2);
		list.add(new Integer(4));
		assertTrue(list.size() == 4);
		assertTrue(list.get(3) == 4);
		list.remove(new Integer(2));
		assertTrue(list.size() == 3);
		assertTrue(list.get(2) == 4);
		assertTrue(list.contains(new Integer(0)));
		
	}
	
	@Test
	public void AddAllTest()
	{
		ArrayList<Integer> test = new ArrayList<Integer>();
		test.add(1);
		test.add(2);
		test.add(3);
		test.add(4);
		LinkedList<Integer> testList = new LinkedList<Integer>();
		testList.addAll(test);
		assertTrue(testList.size() == 4);
		assertTrue(testList.get(0) == 1);
		assertTrue(testList.get(1) == 2);
		assertTrue(testList.get(2) == 3);
		assertTrue(testList.get(3) == 4);
	}
	
	@Test
	public void ClearTest()
	{
		ArrayList<Integer> test = new ArrayList<Integer>();
		test.add(1);
		test.add(2);
		test.add(3);
		test.add(4);
		LinkedList<Integer> testList = new LinkedList<Integer>();
		testList.addAll(test);
		assertTrue(testList.size() == 4);
		testList.clear();
		assertTrue(testList.size() == 0);
	}
	
	@Test
	public void ContainsTest()
	{
		LinkedList<Integer> testList = new LinkedList<Integer>();
		assertFalse(testList.contains(0));
		assertFalse(testList.contains(1));
		assertFalse(testList.contains(2));
		assertFalse(testList.contains(3));
		testList.add(0);
		assertTrue(testList.contains(0));
		assertFalse(testList.contains(1));
		assertFalse(testList.contains(2));
		assertFalse(testList.contains(3));
		testList.add(1);
		assertTrue(testList.contains(0));
		assertTrue(testList.contains(1));
		assertFalse(testList.contains(2));
		assertFalse(testList.contains(3));
		testList.add(2);
		assertTrue(testList.contains(0));
		assertTrue(testList.contains(1));
		assertTrue(testList.contains(2));
		assertFalse(testList.contains(3));
		testList.add(3);
		assertTrue(testList.contains(0));
		assertTrue(testList.contains(1));
		assertTrue(testList.contains(2));
		assertTrue(testList.contains(3));
		testList.remove(0);
		assertFalse(testList.contains(0));
		assertTrue(testList.contains(1));
		assertTrue(testList.contains(2));
		assertTrue(testList.contains(3));
	}
	
	@Test
	public void IndexOfTest()
	{
		LinkedList<Integer> testList = new LinkedList<Integer>();
		assertTrue(testList.indexOf(new Integer(0)) ==  -1);
		assertTrue(testList.indexOf(new Integer(1)) ==  -1);
		assertTrue(testList.indexOf(new Integer(2)) ==  -1);
		assertTrue(testList.indexOf(new Integer(3)) ==  -1);
		testList.add(0);
		assertTrue(testList.indexOf(new Integer(0)) ==  0);
		assertTrue(testList.indexOf(new Integer(1)) ==  -1);
		assertTrue(testList.indexOf(new Integer(2)) ==  -1);
		assertTrue(testList.indexOf(new Integer(3)) ==  -1);
		testList.add(1);
		assertTrue(testList.indexOf(new Integer(0)) ==  0);
		assertTrue(testList.indexOf(new Integer(1)) ==  1);
		assertTrue(testList.indexOf(new Integer(2)) ==  -1);
		assertTrue(testList.indexOf(new Integer(3)) ==  -1);
		testList.add(2);
		assertTrue(testList.indexOf(new Integer(0)) ==  0);
		assertTrue(testList.indexOf(new Integer(1)) ==  1);
		assertTrue(testList.indexOf(new Integer(2)) ==  2);
		assertTrue(testList.indexOf(new Integer(3)) ==  -1);
		testList.add(3);
		assertTrue(testList.indexOf(new Integer(0)) ==  0);
		assertTrue(testList.indexOf(new Integer(1)) ==  1);
		assertTrue(testList.indexOf(new Integer(2)) ==  2);
		assertTrue(testList.indexOf(new Integer(3)) ==  3);
		testList.remove(0);
		assertTrue(testList.indexOf(new Integer(0)) ==  -1);
		assertTrue(testList.indexOf(new Integer(1)) ==  0);
		assertTrue(testList.indexOf(new Integer(2)) ==  1);
		assertTrue(testList.indexOf(new Integer(3)) ==  2);
	}
	
	@Test
	public void RemoveObjectTest()
	{
		LinkedList<Integer> testList = new LinkedList<Integer>();
		testList.add(125);
		testList.add(15453);
		testList.add(0xDEADBEEF);
		testList.add(25);
		testList.remove(new Integer(15453));
		assertTrue(testList.get(1) == 0xDEADBEEF);
	}
	
	@Test
	public void SizeTest()
	{
		LinkedList<Integer> list = new LinkedList<Integer>();
		for(int i = 1; i < 10; i++)
		{
			list.add(new Integer(i));
			assertTrue(list.size() == i);
		}
		list.clear();
		assertTrue(list.size() == 0);
	}
	
	@Test
	public void SetTest()
	{
		LinkedList<Integer> list = new LinkedList<Integer>();
		for(int i = 1; i < 10; i++)
		{
			list.add(new Integer(i));
			list.set(i - 1, new Integer(300));
			assertTrue(list.get(i - 1) == 300);
		}
	}
	
	@Test
	public void isEmptyTest()
	{
		LinkedList<Integer> list = new LinkedList<Integer>();
		assertTrue(list.isEmpty());
		list.add(new Integer(0));
		assertFalse(list.isEmpty());
		list.clear();
		assertTrue(list.isEmpty());
	}

}
