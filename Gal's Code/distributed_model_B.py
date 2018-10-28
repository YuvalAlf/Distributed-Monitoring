# -*- coding: utf-8 -*-
"""
Created on Thu Oct 22 19:54:19 2015

@author: Gal
"""

from __future__ import division
from random import randint
import networkx as nx
import matplotlib.pylab as plt
import numpy as np
import scipy.stats as stats
from sparseDistributed import findGap, findl1, findl2
from checkC2Approximation import gapBound
import scipy as sp
from distributed_model_A import generate_split_matrix
import matplotlib.patches as mpatches



def split_initial_graph2(G, proc):
    subMatrices = []
    for i in range(proc):
        tmp = (1 / proc)*nx.to_numpy_matrix(G)
        subMatrices.append(tmp)
    return subMatrices
    
def add_edge_dist2(Gx, dist, local_matrices, A):
    x = Gx.nodes()
    custm = stats.rv_discrete(name='custm', values=(x, dist))
    edge = custm.rvs(size=2)
    '''
    while(Gx.has_edge(edge[0], edge[1])):
     edge = custm.rvs(size=2)    
     '''
    local_matrices[A[edge[0]][edge[1]]][edge[0], edge[1]] = 1
    local_matrices[A[edge[0]][edge[1]]][edge[1], edge[0]] = 1
    Gx.add_edge(edge[0],edge[1]) 
    return (A[edge[0]][edge[1]])
    
def remove_edge_dist2(Gx, dist, local_matrices, A):
    k = randint(0, len(local_matrices) - 1)
    edges = np.transpose(np.nonzero(local_matrices[k]))
    if (len(edges) == 0):
        return k
    edge = edges[randint(0, len(edges) - 1)]
    while (Gx.has_edge(edge[0][0], edge[0][1]) == False):
        edge = edges[randint(0, len(edges) - 1)]
    Gx.remove_edge(edge[0][0], edge[0][1])
    tmp = local_matrices[k][edge[0][0], edge[0][1]]
    if (tmp == 1):
        local_matrices[k][edge[0][0], edge[0][1]] = 0
        local_matrices[k][edge[0][1], edge[0][0]]
    else:
        local_matrices[k][edge[0][0], edge[0][1]] = local_matrices[k][edge[0][0], edge[0][1]] - 1
        local_matrices[k][edge[0][1], edge[0][0]] = local_matrices[k][edge[0][1], edge[0][0]] - 1
    return k
    

def random_distributed_change2(G, A, time, local_matrices, dist, u0 = None, proc = 10):
    T = 22.6
    
    timeAxis = [0]
    noOfEdges = nx.number_of_edges(G)
    gap_bound = [np.real(gapBound(nx.to_numpy_matrix(G), u0).item(0))]
    print (gap_bound[0])
    global_gap = [findGap(nx.to_numpy_matrix(G))]
    print (global_gap[0])
    SUM = 0 
    GAPS = []
    BSUM = 0
    BGAPS = []
    e10s = []
    
    for i in local_matrices:
        shit, utmp = sp.sparse.linalg.eigs(i, k = 1)
        e10s.append(utmp.T[0])
    
    for i in range(0, len(local_matrices)):
        tmp = np.real(gapBound(local_matrices[i], e10s[i]).item(0))
        BSUM += tmp
        BGAPS.append(tmp)
    
    for i in local_matrices:
        SUM += findGap(i)
        GAPS.append(findGap(i))
        
    local_gap = [SUM]
    local_bound = [BSUM]
    gv =[0]
    lv = [0]
    global_violations = 0
    local_violations = 0
    
    for i in range(1, time):
        print (i)
        coin = randint(0,1)
        if (coin == 1):
            k = add_edge_dist2(G, dist, local_matrices, A)
            timeAxis.append((i/noOfEdges)*100)
            gap_bound.append(np.real(gapBound(nx.to_numpy_matrix(G), u0).item(0)))
            global_gap.append(findGap(nx.to_numpy_matrix(G)))
            GAPS[k] = findGap(local_matrices[k])
            BGAPS[k] = np.real(gapBound(local_matrices[k], e10s[k]).item(0))        
            local_gap.append(sum(GAPS))
            local_bound.append(sum(BGAPS))
            print (BGAPS[k])
            for j in range(proc):
                if(BGAPS[j] <= T/proc):
                    local_violations += 1
                    break
            if (global_gap[i] <= T):
                global_violations += 1
            gv.append(global_violations)
            lv.append(local_violations)
            print(global_gap[i], gv[i],lv[i])
            
        else:
            k = remove_edge_dist2(G, dist, local_matrices, A)
            timeAxis.append((i/noOfEdges)*100)
            gap_bound.append(np.real(gapBound(nx.to_numpy_matrix(G), u0).item(0)))
            global_gap.append(findGap(nx.to_numpy_matrix(G)))
            GAPS[k] = findGap(local_matrices[k])
            BGAPS[k] = np.real(gapBound(local_matrices[k], e10s[k]).item(0)) 
            print (BGAPS[k])
            local_gap.append(sum(GAPS))
            local_bound.append(sum(BGAPS))
            for j in range(proc):   
                if(BGAPS[j] <= T/proc):
                    local_violations += 1
                    break
            if (global_gap[i] <= T):
                global_violations += 1
            gv.append(global_violations)
            lv.append(local_violations)
            print(global_gap[i], gv[i],lv[i])

    res = [timeAxis, global_gap, local_gap, gap_bound, local_bound, gv, lv]
    return res
    

def simple_add(Gx, dist):
    x = Gx.nodes()
    custm = stats.rv_discrete(name='custm', values=(x, dist))
    edge = custm.rvs(size=2)
    while(Gx.has_edge(edge[0], edge[1])):
     edge = custm.rvs(size=2)    
    Gx.add_edge(edge[0],edge[1]) 
    
def simple_del(Gx):
    edges = Gx.edges()
    if (len(edges) == 0):
        return 
    edge = edges[randint(0, len(edges) - 1)]
    while (Gx.has_edge(edge[0], edge[1]) == False):
        edge = edges[randint(0, len(edges) - 1)]
    Gx.remove_edge(edge[0], edge[1])



def simple_change(G, time, dist, u0 = None):
    timeAxis = [0]
    noOfEdges = nx.number_of_edges(G)
    gap_bound = [np.real(gapBound(nx.to_numpy_matrix(G), u0).item(0))]
    print (gap_bound[0])
    global_gap = [findGap(nx.to_numpy_matrix(G))]
    print (global_gap[0])

    for i in range(1, time):
        print (i)
        for j in range(2):
            coin = randint(0,1)
            if (coin == 1):
                simple_add(G, dist)
            else:
                simple_del(G)
                
        timeAxis.append(((i*100)/noOfEdges)*100)
        gap_bound.append(np.real(gapBound(nx.to_numpy_matrix(G), u0).item(0)))
        global_gap.append(findGap(nx.to_numpy_matrix(G)))
       	print("GAP:", global_gap[i], "BOUND:",gap_bound[i]) 
    res = [timeAxis, global_gap,  gap_bound]
    return res



def time_to_hit(local_matrices, dist, f, u0 = None, proc = 10):
    time = 0
    while (True):
        pass   
    pass


def expected_time_to_fail(G, A, time, local_matrices, dist, f, u0 = None, proc = 10):
    #Average time to hit
    #Precent of change
    timeToHitBound = []
    timeToHitGap = []
    pOfChange = []
    
    noOfEdges = nx.number_of_edges(G)
    
    for i in range(time):
        timeToHitGap.append(time_to_hit(local_matrices, dist, findGap, u0, proc))
        timeToHitBound.append(time_to_hit(local_matrices, dist, gapBound, u0, proc))
        pOfChange.append((i/noOfEdges)*100)
        
    res = [pOfChange, timeToHitGap, timeToHitBound]
    return res
     

    
    
def normalized(a, axis=-1, order=2):
    l2 = np.atleast_1d(np.linalg.norm(a, order, axis))
    l2[l2==0] = 1
    return a / np.expand_dims(l2, axis)

def main():
    print ("START")
    #Creating a graph
    print("CREATE GRAPH")
    #G = nx.powerlaw_cluster_graph(100, 10, 0.2)
    
    #G = nx.gnm_random_graph(1000, 25000)
    G = nx.star_graph(1000) 
    '''
    H = nx.star_graph(10)
    F = nx.compose(G,H)
    print("CREATE GRAPH - DONE")
    
    
    degree_sequence=sorted(nx.degree(F).values(),reverse=True)
    
    
    #print (max(degree_sequence))
    print(degree_sequence[0], degree_sequence[1])
    print (findl1(nx.to_numpy_matrix(F))) 
    print(findl2(nx.to_numpy_matrix(F)))
    
    
    '''
       
    
    #First par. = |V|. Se.par = n.o. proc.
    #print("SPLIT MATRIX")
    #A = generate_split_matrix(100, 10)
    #print("SPLIT MATRIX - DONE")    
    
    dist2 = [1 / nx.number_of_nodes(G)] * nx.number_of_nodes(G)
    #dist = nx.utils.powerlaw_sequence(501, 2.5)
    
    #print("SPLIT GRAPHS")
    #local_graphs = split_initial_graph2(G, 10)
    #print("SPLIT GRAPHS - DONE")
    
    d1_0, e1_0 = sp.sparse.linalg.eigs(nx.to_numpy_matrix(G), k = 1)
    u0 = e1_0.T[0]
    
    print("LET THE FUN BEGIN:")
    #res = random_distributed_change2(G, A, 300, local_graphs, dist2, u0, proc = 10)
    
    res = simple_change(G, 750, dist2, u0)
    
    #res[1] = normalized(res[1])
    #res[2] = normalized (res[2])
    
    np.save("timeAxis2", res[0])
    np.save("gapAxis2", res[1])
    np.save("boundAxis2", res[2])




main()



def here_for_historical_reasons():
    fig = plt.figure(120)
    plt.rc('xtick', labelsize=40) 
    plt.rc('ytick', labelsize=40) 
    font = {'family' : 'normal',
        'weight' : 'bold',
        'size'   : 40}

    plt.rc('font', **font) 

    #plt.plot(res[0], res[1], label='global gap', linewidth= 5)
    #plt.plot(res[0], res[2], label = 'sum of local \n gaps', linewidth= 5)
    '''    
    a = res[5]
    b = res[6]
    c = []
    for i in range(len(a)):
        if (b[i] == 0):
            c.append(0)
        else:
            c.append(a[i] / b[i])
    #plt.plot(res[0],c, label = 'gv/lv', linewidth= 5)
    plt.plot(res[0],a,label='global violations', linewidth= 5)
    plt.plot(res[0], b, label='local violations', linewidth= 5)
    '''
    plt.plot(res[0], res[1], label="gap",linewidth= 5)
    plt.plot(res[0], res[2], label="bound",linewidth= 5)
    plt.xlabel("% of change")
    lgd = plt.legend(bbox_to_anchor=(0., 1.02, 1., .102), loc=3,
       ncol=2, mode="expand", borderaxespad=0.)  
    fig.savefig('samplefigure2', bbox_extra_artists=(lgd,), bbox_inches='tight',pad_inches=0.1)
    
    
    
        
    #plt.figure(20)
    #plt.plot(res[0], res[3])
    #plt.title("Sum of Local Gaps")
    #plt.xlabel("Time")
    #plt.ylabel("Gap")
    #plt.figure(30)
    #plt.plot(res[0], res[4])
    #plt.title("Sum of Local Bounds")
    #plt.xlabel("Time")
    #plt.ylabel("Bound")
    





    
