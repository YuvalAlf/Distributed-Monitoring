import cvxpy as cvx

def printTab (text1, text2):
    print('\t' + text1.__str__() + " = " + text2.__repr__())

class prettyFloat(float):
    def __repr__(self):
        return "%0.3f" % self

def toPrettyFloat (vector):
    return list(map(prettyFloat, vector)).__repr__()

def minimizeL1(functionName, function, initialVector, thresholdValue):
    dimension = len(initialVector)
    distanceVector = cvx.Variable(dimension)
    point = cvx.Parameter(dimension)
    point.value = initialVector
    curvature = function(point + distanceVector).curvature
    constraint = function(point + distanceVector) <= thresholdValue \
                 if curvature == "CONVEX" \
                 else function(point + distanceVector) >= thresholdValue
    objective = cvx.Minimize(cvx.norm1(distanceVector))
    problem = cvx.Problem(objective, [constraint])

    print(functionName + ":")
    printTab("Initial vector", toPrettyFloat(initialVector))
    printTab("Function value", function(initialVector).value)
    printTab("Threshold", thresholdValue)
    printTab("Curvature", curvature)

    problem.solve()

    printTab("Solve status", problem.status)
    printTab("Minimal L1 distance", problem.value)
    printTab("Distance vector", toPrettyFloat(distanceVector.value))
    printTab("Vector on threshold", toPrettyFloat((initialVector + distanceVector).value))
    print()

def minusLogSumExp(vector):
    return -cvx.log_sum_exp(vector)

def normL2Squared(vector):
    return cvx.sum_squares(vector)

def concave1(vector):
    return 10 * cvx.sqrt(vector[0] + vector[1]) - cvx.square(vector[1] + vector[2]) + cvx.log(vector[0] + vector[2])

def convex2(vector):
    return cvx.square(vector[0]) + cvx.norm(vector, 2)



if __name__ == '__main__':
    #minimizeL1("minusLogSumExp", minusLogSumExp, [-1, -2, -3, -4, -5, -6, -7, -8, -9, -10], 2.0)
    #minimizeL1("normSquared", normL2Squared, [6, 8], 10)
    #minimizeL1("specialConcaveFunction", concave1, [20, 5, 2], 100)
    minimizeL1("specialConvexFunction", convex2, [3, 3], 1)
