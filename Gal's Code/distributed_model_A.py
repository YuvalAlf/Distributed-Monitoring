# -*- coding: utf-8 -*-
"""
Created on Tue Oct 20 09:19:05 2015

@author: Gal
"""
from random import randint
import networkx as nx
import matplotlib.pylab as plt
import numpy as np
import scipy.stats as stats
from sparseDistributed import findGap, findl1
from checkC2Approximation import gapBound
import scipy as sp


def generate_split_matrix(size, proc):
    A = []
    for i in range(size):
        tmp = []
        for j in range(size):
            tmp.append(randint(0, proc - 1))
        A.append(tmp)
    return A

def split_initial_graph(G, A, proc):
    subGraphs = []    
    for i in range(0, proc):
        H = nx.Graph()
        H.add_nodes_from(G.nodes())
        subGraphs.append(H) 
    
    global_edges = G.edges()
    
    for i in global_edges:
        subGraphs[A[i[0]][i[1]]].add_edge(*i)
    
    return subGraphs

def add_edge_dist(Gx, dist, local_graphs, A):
    x = Gx.nodes()
    custm = stats.rv_discrete(name='custm', values=(x, dist))
    edge = custm.rvs(size=2)
    while(Gx.has_edge(edge[0], edge[1])):
      edge = custm.rvs(size=2)
    local_graphs[A[edge[0]][edge[1]]].add_edge(edge[0],edge[1])
    Gx.add_edge(edge[0],edge[1]) 
    return (A[edge[0]][edge[1]])
    
def remove_edge_dist(Gx, dist, local_graphs, A):
    k = randint(0, len(local_graphs) - 1)
    edges = local_graphs[k].edges()
    if (len(edges) == 0):
        return k
    edge = edges[randint(0, len(edges) - 1)]
    Gx.remove_edge(*edge)
    local_graphs[k].remove_edge(*edge)
    return k
    
    
    
def random_distributed_change(G, A, time, local_graphs, dist, u0 = None):
    timeAxis = [0]
    gap_bound = [np.real(gapBound(nx.to_numpy_matrix(G), u0).item(0))]
    print (gap_bound[0])
    global_gap = [findGap(nx.to_numpy_matrix(G))]
    print (global_gap[0])
    SUM = 0 
    GAPS = []
    BSUM = 0
    BGAPS = []
    e10s = []
    for i in local_graphs:
        #shit, utmp = sp.sparse.linalg.eigs(nx.to_numpy_matrix(G), k = 1)
        shit, utmp = sp.sparse.linalg.eigs(nx.to_numpy_matrix(i), k = 1)
        e10s.append(utmp.T[0])
    
    for i in range(0, len(local_graphs)):
        tmp = np.real(gapBound(nx.to_numpy_matrix(local_graphs[i]), e10s[i]).item(0))
        BSUM += tmp
        BGAPS.append(tmp)
    
    for i in local_graphs:
        SUM += findGap(nx.to_numpy_matrix(i))
        GAPS.append(findGap(nx.to_numpy_matrix(i)))
        
    local_gap = [SUM]
    local_bound = [BSUM]
    
    for i in range(1, time):
        print (i)
        coin = randint(0,1)
        if (coin == 1):
            k = add_edge_dist(G, dist, local_graphs, A)
            timeAxis.append(i)
            gap_bound.append(np.real(gapBound(nx.to_numpy_matrix(G), u0).item(0)))
            global_gap.append(findGap(nx.to_numpy_matrix(G)))
            GAPS[k] = findGap(nx.to_numpy_matrix(local_graphs[k]))
            BGAPS[k] = np.real(gapBound(nx.to_numpy_matrix(local_graphs[k]), e10s[k]).item(0))        
            local_gap.append(sum(GAPS))
            local_bound.append(sum(BGAPS))
        else:
            k = remove_edge_dist(G, dist, local_graphs, A)
            timeAxis.append(i)
            gap_bound.append(np.real(gapBound(nx.to_numpy_matrix(G), u0).item(0)))
            global_gap.append(findGap(nx.to_numpy_matrix(G)))
            GAPS[k] = findGap(nx.to_numpy_matrix(local_graphs[k]))
            BGAPS[k] = np.real(gapBound(nx.to_numpy_matrix(local_graphs[k]), e10s[k]).item(0)) 
            local_gap.append(sum(GAPS))
            local_bound.append(sum(BGAPS))

            
    res = [timeAxis, global_gap, local_gap, gap_bound, local_bound]
    return res



def main():
    G = nx.gnm_random_graph(1000, 20000)
    
    '''
    A = generate_split_matrix(1000, 10)
    dist2 = [1 / nx.number_of_nodes(G)] * nx.number_of_nodes(G)
    local_graphs = split_initial_graph(G, A, 10)
    d1_0, e1_0 = sp.sparse.linalg.eigs(nx.to_numpy_matrix(G), k = 1)
    u0 = e1_0.T[0]
    res = random_distributed_change(G, A, 2000, local_graphs, dist2, u0)
    plt.figure(1)
    plt.plot(res[0], res[1], res[0], res[3])
    plt.title("Global Gap and Global Bound")
    plt.xlabel("Time")
    plt.ylabel("Gap, Bound")
    plt.figure(2)
    plt.plot(res[0], res[2], res[0], res[4])
    plt.title("Sum of Local Gaps")
    plt.xlabel("Time")
    plt.ylabel("Gap")
    plt.figure(3)
    plt.plot(res[0], res[4])
    plt.title("Sum of Local Bounds")
    plt.xlabel("Time")
    plt.ylabel("Bound")
    '''



#main()