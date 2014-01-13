"""
This module implements a Wavelet tree
"""

import time
import sys
import resource

class WaveletTree(object):
    """
    This class implements Wavelet tree structure
    """
    def __init__(self, string):
        """
        Initialization and tree construction
        """
        self.string = string
        self.root_node = WaveletNode(None)
        self.alphabet = []

        self.create_alphabet()  
        self.root_node.create_tree(self.string, self.alphabet)

    def rank(self, position, character):
        """
        Delegates rank method to node
        """
        return self.root_node.rank(position, character, self.alphabet)

    def member(self, index):
        """
        Returns character at 'index' in a string
        """
	index -= 1
        alphabetic_interval = Interval(0, len(self.alphabet) - 1)
       
        if (index > len(self.root_node.bit_vector) - 1):
            return "Index out of range!"
        else:
            current_node = self.root_node
            new_element_position = index

            while(alphabetic_interval.is_greater_than_two()):
                if current_node is not None:
                    index = new_element_position
                    if (current_node.bit_vector[index] == '1'):
                        # count 1s up to index 'position'
                        new_element_position =  current_node.bit_vector.count('1', 0, index)
                        current_node = current_node.right;
                        alphabetic_interval.set_left_index()

                    else:
                        #count 0s
                        new_element_position = index - current_node.bit_vector.count('1', 0, index)
                        current_node = current_node.left;
                        alphabetic_interval.set_right_index()
		else:
	 	    break

            if (current_node is not None):
                if (current_node.bit_vector[new_element_position] == '1'):
                    return self.alphabet[alphabetic_interval.right_index]
                else:
                    return self.alphabet[alphabetic_interval.left_index]
            else:
                return self.alphabet[alphabetic_interval.left_index]

		
    def create_alphabet(self):
        """
        Get alphabet from input string
        """
        for char in self.string:
            if char not in self.alphabet:
                self.alphabet.append(char)
        self.alphabet.sort()

    def select (self, nth_occurrence, character):
        """
        Returns position in string where character occurs for the nth time
        """

        alphabetic_interval = Interval(0, len(self.alphabet) - 1)
        current_node = self.root_node

        try:
            char_index = self.alphabet.index(character)
        except:
            return "Character doesn't exist"

        zero_character = True

        while (alphabetic_interval.is_greater_than_two()):
            # Special case, node with 3 bits
            # Left child node exists, but right one doesn't
            if (alphabetic_interval.get_size() == 3):
                if(alphabetic_interval.right_index == char_index):
                    zero_character = False
                    break

            # Going into left node	
            if (char_index <= alphabetic_interval.get_middle_index()):
                current_node = current_node.left
                alphabetic_interval.set_right_index()
            # Right node
            else:
                current_node = current_node.right
                alphabetic_interval.set_left_index()

        # Setting values for unset zero_character
        if zero_character:
            if (alphabetic_interval.left_index == char_index):
                zero_character = True
            else:
                zero_character = False

        position = self.get_position_of_nth_occurrence(current_node.bit_vector,
                                                       nth_occurrence, zero_character)

        if (not position):
            return "Character \ndoesn't have that many occurrences"
				
        child = current_node
        current_node = current_node.parent

	# Going up the tree through parent node
        while(current_node is not None):
            if (current_node.left == child):
                position = self.get_position_of_nth_occurrence(current_node.bit_vector,
                                                               position, True);
                
            else:
                position = self.get_position_of_nth_occurrence(current_node.bit_vector,
                                                               position, False);
			
            current_node = current_node.parent
            child = child.parent		
		
        return position
    

    def get_position_of_nth_occurrence(self, bit_vector,
                                       nth_occurrence, zero_character):
        """
        Returns index in bit_vector where
        zero_character (0 or 1) occurs for the nth time 
        """
        counter = 0
        position = 0

        for c in bit_vector:
            if counter < nth_occurrence:
                position += 1

                if (zero_character and c == '0'):
                    counter += 1
                elif (not zero_character and c == '1'):
                    counter += 1
            else:
                break
                    
        if (counter == nth_occurrence):
            return position
        else:
            return 0
    

class WaveletNode(object):
    """
    This class implements a node in a Wavelet tree
    """
    def __init__(self, parent):
        self.bit_vector = ""
        self.left = None
        self.right = None
        self.parent = parent

    def create_tree(self, string, alphabet):
        """
        Creates Wavelet Tree for input string
        """
        left_string = ""
        right_string = ""
        
        middle = self.alphabet_middle(alphabet)
        left_alphabet = alphabet[0 : middle]
        right_alphabet = alphabet[middle: ]

        # If alphabet length is > 2, then we're in a node, leaf otherwise
        if len(alphabet) > 2:
            # Creating filtered strings for left and right node
            for char in string:               
                if char in left_alphabet:
                    left_string += char
                    self.bit_vector += "0"
                else:
                    right_string += char
                    self.bit_vector += "1"
            
            # Creating new nodes and recursively calling create_tree method
            self.left = WaveletNode(self)
            self.left.create_tree(left_string, left_alphabet)

            # If alphabet is equal to 3, then we're creating just left leaf
            if len(alphabet) != 3:
                self.right = WaveletNode(self)
                self.right.create_tree(right_string, right_alphabet)            

        else:
            # This is a leaf
            for char in string:
                bit_value = '0' if char in left_alphabet else '1'
                self.bit_vector += bit_value

    def rank(self, index, character, alphabet):
        """Returns the number of occurences
        of the character up to the specified index in the string.
        """

        if character not in alphabet:
            return 0

        if index > len(self.bit_vector) - 1:
            return "Index out of range"

        bit_counter = 0
        
        left_alphabet = self.get_left_alphabet(alphabet)
        right_alphabet = self.get_right_alphabet(alphabet)

        # Determine if character is in left or right alphabet
        bit = '0' if character in left_alphabet else '1'
        # Slice alphabet to left or right one
        alphabet_sliced = left_alphabet if bit == '0' else right_alphabet

        # Counting the number of the same bit values as the character's bit
        for char in self.bit_vector[0 : index + 1]:
            if char == bit:
                bit_counter += 1

        node = self.left if bit == '0' else self.right
        if node is not None:
            return node.rank(bit_counter - 1, character, alphabet_sliced)
        else:
            return bit_counter

    def alphabet_middle(self, alphabet):
        """
        Get middle index of the alphabet
        """
        return ((len(alphabet) + 1) / 2)

    def get_left_alphabet(self, alphabet):
        """
        Cuts alphabet in half and gets left part
        
        Notice: if alphabet's length is an odd number,
        left part gets one character more
        """
        middle = self.alphabet_middle(alphabet)
        return alphabet[0 : middle]
    
    def get_right_alphabet(self, alphabet):
        """
        Cuts alphabet in half and gets right part
        
        Notice: if alphabet's length is an odd number,
        left part gets one character less
        """
        middle = self.alphabet_middle(alphabet)
        return alphabet[middle : ]

class Interval(object):
    """
    This class handles alphabet indices as an interval while traversing child nodes
    """
    def __init__(self, left_index, right_index):
        """
        Sets starting and ending index
        """
        self.left_index = left_index
        self.right_index = right_index

    def set_left_index(self):
        """
        Set left index while going into right node
        """
        self.left_index = self.right_index - (self.get_size()/2 - 1)

    def set_right_index(self):
        """
        Set right index while going into left node
        """
        self.right_index = self.left_index + ((self.get_size() + 1) / 2 - 1)

    def get_size(self):
        """
        Get interval size
        """
        return self.right_index - self.left_index + 1;

    def is_greater_than_two(self):
        """
        Check if interval is bigger than 2
        """
        if self.right_index - self.left_index > 1:
            return True
        else:
            return False
        
    def get_middle_index(self):
        """
        Get index in the middle of the interval
        """
        return (self.left_index + self.right_index) / 2
        
     

class Fasta(object):
    """
    This class loads fasta file
    """
    def __init__(self):
        """Initialization"""
        self.data = ""
        self.length = -1
        self.meta_data = "no meta-data"

    def load(self, conn_str):
        """
        Loads fasta file into self.data without newlines,
        comments and headers
        """
        
        data = open(conn_str, 'r')
        header = data.readline().strip()

        #ignoring header and comments
        if(header[0] == '>'):
            self.meta_data = header[1:]
        elif header[0] == ';':
            pass
        else:
            self.data += header.upper()

        for line in data:
            if line[0] != ';':
                self.data += line.strip().upper()

        self.length = len(self.data)


def get_memory():
    """Returns max memory usage of the process"""
    return resource.getrusage(resource.RUSAGE_SELF).ru_maxrss

def used_time(start_time):
    """Prints difference in start_time and time.now"""
    return str((time.time() - start_time) * 1000) + " ms"

def select(index, char):
    """Calls select function and prints results and execution time"""
    occurrence_time = time.time()
    print(str(index) + ". occurrence of character " + char + " is at position (SELECT): " + str(tree.select(index, char)))
    print("Executed in: " + used_time(occurrence_time))
    print

def rank(position, char):
    """Calls rank function and prints results and execution time"""
    rank_time = time.time()
    print("Rank of the character " + char + " up to position " + str(position)+ " is (RANK): " + str(tree.rank(position, char)))
    print("Executed in: " + used_time(rank_time))
    print

def member(index):
    """Calls member function and prints results and execution time"""
    member_time = time.time()
    print("Character at position " + str(index) + " is (MEMBER): " + tree.member(index))
    print("Executed in: " + used_time(member_time))
    print

def tree_construction_time():
    """Prints tree construction time"""
    print "Meta data: " + str(genome.meta_data)
    print("Tree created in: " + used_time(tree_time))	
    print
    print("Length of the genome is: " + str(genome.length) + " characters")
    print

def data_load_time():
    """Prints data loading time"""
    print "Data loaded in: " + used_time(load_time)
    print
    print "Fasta file: " + file_path
    print "----------"

def final_data():
    """Prints memory usage and total execution time"""
    print "Script executed in: " + used_time(start_time)
    print "Total memory usage: " + str(get_memory()) + " kB"
    print

if __name__ == "__main__":
        
    try:
        start_time = time.time()
	
	if(len(sys.argv) != 7):
            raise Exception("Broj argumenata mora biti 6!")

	rank_char = str(sys.argv[2]).upper()
	rank_position = int(sys.argv[3])
	
	select_occ = int(sys.argv[4])
	select_char = str(sys.argv[5]).upper()

	member_index = int(sys.argv[6])

        file_path = sys.argv[1]

        print

        genome = Fasta()

        load_time = time.time()
        genome.load(file_path)
        data_load_time()

        tree_time = time.time()
        tree = WaveletTree(genome.data)
        tree_construction_time()

        rank(rank_position, rank_char)
	select(select_occ, select_char)
        member(member_index)


        final_data()
        
    except IOError:
        print "File doesn't exist or wrong path specified"

    except Exception, e:
        print "Unknown error occurred: " + str(e)
	

    
