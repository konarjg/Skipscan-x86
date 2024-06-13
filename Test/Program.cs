using Test;

var nodes = new List<Node>();

nodes.Add(new Node("mov $0, %rax", 1, "A"));
nodes.Add(new Node("mov $0, %rbx", 1, "B"));
nodes.Add(new Node("mov $0, %rcx", 1, "C"));
nodes.Add(new Node("mov $0, %rdx", 1, "D"));
nodes.Add(new Node("and %rax, %rbx", 2, "E"));
nodes.Add(new Node("add %rcx, %rdx", 2, "F"));
nodes.Add(new Node("jz label", 3, "G"));

var graph = new Graph(nodes);

Console.WriteLine(graph);
graph.Sort();
Console.WriteLine(graph);
