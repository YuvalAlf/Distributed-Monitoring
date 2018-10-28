# -*- coding: utf-8 -*-
"""
Created on Sun Feb 22 20:12:20 2015

@author: Gal
"""

# Given vector x0, Generate matrix R such that Rx0 = (0,0,0...,1) and R is 
# rotate matrix

import numpy as np
import math as m

    
    


print("BB")

#v1a = [0.5, 0.5, 1/m.sqrt(2)]
#v1 = np.array(v1a)
#print ("AAAA")

#print(v1)

def generateStandardBasis(v1, dim): 
    vectors = []
    vectors.append(v1)
    i = 1
    
    while (i <= dim):
        j = 1
        v = []
        while (j <= dim):
            if (j == i):
                v.append(1)
            else:
                v.append(0)
            j = j + 1
        vi = np.array(v)
        vectors.append(vi)
        i = i + 1
        
    return vectors
    
    
def generateR(vectors, v1, dim):
    R = []
    R.append(v1)
    zeroVec = np.zeros(dim)
    i = 1
    while (i <= dim):
        w1 = vectors[i]
        w = np.array(w1)
        j = 0 
        while (j < i):
            dotP = np.dot(vectors[i], R[j])
            vec = np.multiply(dotP, R[j])
            w = w - vec
            j = j + 1
        if ((np.array_equal(zeroVec, w)) == True):
            R.append(zeroVec)
        if ((np.array_equal(zeroVec, w)) == False):
            normV = 1 / (np.linalg.norm(w))
            u = np.multiply(normV, w)
            R.append(u)
        i = i + 1
    removearray(R, zeroVec)
    R.reverse()
    R = np.matrix(R)
    return R
    
    
def removearray(L,arr):
    ind = 0
    size = len(L)
    while ind != size and not np.array_equal(L[ind],arr):
        ind += 1
    if ind != size:
        L.pop(ind)
    else:
        L.pop()


def start(u0):
    #v1 = np.load("ee1.npy")
    v1 = u0
#    print("||x0|| = ", np.linalg.norm(v1))
    dim = np.size(v1)
    v = generateStandardBasis(v1, dim)
    f = generateR(v, v1, dim)
#    ft = np.transpose(f)
    np.save("R.npy", f)
    return f




#print (np.dot(f[1],f[0].T))
#print(np.dot(ft,f))


