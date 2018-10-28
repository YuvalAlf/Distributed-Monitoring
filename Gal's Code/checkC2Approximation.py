# -*- coding: utf-8 -*-
"""
Created on Fri Aug 21 14:11:25 2015

@author: Gal
"""

from generateRotateMAtrix import start
import numpy as np
import scipy as sp
import matplotlib.pylab as plt
from sparseDistributed import findl1, generateA0
import networkx as nx    
    
    
        
        
def bestBound(A, R):
    G = np.dot(R, A)
    G = np.dot(G, R.T)
    H = np.delete(G, len(G) - 1, axis = 1)
    H = np.delete(H, len(G) - 1, axis = 0)
    dA, eA = np.linalg.eig(H)
    return max(dA)    
    
    
def c2(A, u0):
    R = start(u0)
    return (bestBound(A, R))
    
def l1bound(A, u0):
    return (np.dot(np.dot(A, u0), u0))
    
def A2(A, u0):
    B = np.outer(u0, u0)
    return (findl1(A - np.dot(B, A)))
    
def P(z, u0):
    x = np.inner(z, u0)
    tmp = x*u0
    return (z - tmp)
    
def sP(z, u0):
    u0 = sp.sparse.csr_matrix(u0)
    u0 = u0.T
    x = z.dot(u0.T)
    tmp = x*u0
    return (z - tmp)

ITERATIONS = 10
def powerA2(A, u0):
    A = np.array(A)
    x0 = np.random.rand(len(A))
    for i in range(ITERATIONS):
        x0 = P(np.dot(A, x0), u0)
        x0 = x0 / np.linalg.norm(x0)
    return (np.inner(np.dot(A, x0), x0))


def spowerA2(A, u0):
    x0 = sp.sparse.rand(A.shape[0], 1)
    for i in range(ITERATIONS):
        x0 = sP(A.dot(x0), u0)
        x0 = x0 / np.linalg.norm(x0.toarray())
    tmp = A.dot(x0)
    return (tmp.dot(x0.T))


def try2(A, u0):
    U0  = np.outer(u0, u0)
    tmp1 = np.dot(A, U0)
    tmp2 = np.dot(U0, A)
    scalar = np.inner(np.dot(A, u0), u0)
    matrix = A - tmp1 - tmp2 + scalar*U0
    return (findl1(matrix))
    
    
# u0 = e1(0), A = G(T)
def gapBound(A, u0):
    l1 = l1bound(A, u0)
    l2 = c2(A, u0)
    if (l1 < 0 and l2 > 0 ):
        return (-1*l1 - l2)
    else:
        return (l1 - l2)
    
    
def TEST():
    t = []
    C2 = []
    power = []
    l = []
    j = []
    a = 0
    for i in range(10):
        print (i)
        A = np.random.rand(1000,1000)
        #g = nx.gnm_random_graph(1000, 1000)
       # A = sp.sparse.rand(1000, 1000, 0.01)
        #A = A.toarray()
        #print(type(A))
        A = np.dot(A, A.T)        #
        #A = nx.to_numpy_matrix(g)
        u0 = np.random.rand(1000)
        #d1_0, e1_0 = sp.sparse.linalg.eigs(A, k=1)
        #u0 = e1_0.T[0]
        u0 = u0 / np.linalg.norm(u0)
        #A = sp.sparse.coo_matrix(A)
        #print (powerA2(A, u0))
#
#        print (u0)            
        
#        Graph = nx.gnm_random_graph(100,100)
#        d1_0, e1_0 = sp.sparse.linalg.eigs(nx.to_numpy_matrix(Graph), k = 1)
#        A = nx.to_numpy_matrix(Graph)
#        u0 = e1_0.T[0]  
#        u0 = u0 / np.linalg.norm(u0)
        
        t.append(i)
        C2.append(c2(A, u0))
        #power.append(powerA2(A, u0))
        #l.append(A2(A, u0))
        #j.append(try2(A, u0))
        print (C2[i])
        #print(power[i])
        #print (j[i])

    plt.plot(t, C2)
        
    
    
    
#TEST()
    
    
    
#for i in range(10):
##    Graph = nx.gnm_random_graph(10,40)
##    d1_0, e1_0 = sp.sparse.linalg.eigs(nx.to_numpy_matrix(Graph), k = 1)
#    x0 = np.random.rand(5)
#    x0 = x0 / np.linalg.norm(x0)
#    A = np.random.rand(5, 5)
#    A = np.dot(A, A.T)
##    A = nx.to_numpy_matrix(Graph)
##    x0 = e1_0.T[0]
#    print ("A2 = ", A2(A, x0), "; c2 = ", c2(A, x0), "; power = ", powerA2(A, x0))
    
    
    
