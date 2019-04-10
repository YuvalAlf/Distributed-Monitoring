import random
import sys
import cvxpy as cvx
import numpy

def distanceL1FromCircle(threshold, point, solver, options):
    point = numpy.array(point, dtype=float)
    x = cvx.Variable(len(point))
    objective = cvx.Minimize(cvx.norm(x - point, 1))
    constraints = [cvx.norm(x, 2) <= numpy.sqrt(threshold)]
    prob = cvx.Problem(objective, constraints)
    try:
        prob.solve(solver=solver, **options)
        # Ignore suboptimal solutions
        if prob.status != 'optimal':
            return '-', prob.status
        solution = numpy.asarray(x.value).squeeze()
        distanceVector = [0 if abs(val) < 1e-5 else val for val in point - solution]
        l0 = sum([val != 0 for val in distanceVector])
        return l0, 'optimal'
    except cvx.SolverError:
        return '-', 'failed'

def realDimensionReduction(point, threshold):
    sqr = lambda x: x * x
    pointSorted = sorted(point, key=abs, reverse=True)
    sumSquarred = sum(sqr(x) for x in point)
    dimension = 0
    while True:
        if sumSquarred <= threshold:
            return dimension
        sumSquarred -= sqr(pointSorted[dimension])
        dimension += 1


if __name__=='__main__':
    # Solvers and parameters
    solver, solverOptions = 'SCS', dict(eps=1e-7, max_iters=1000000)
    print('Dimension;Ratio;L0 of L1 Distance;Real L0 Distance')
    #for dimension in numpy.linspace(100, 1000, 11): # 91
    for dimension in [100000]:
        point = [random.uniform(0.0, 1.0) for _ in range(dimension)]
        pointFunctionValue = sum([v * v for v in point])
        for ratio in numpy.linspace(0.9, 0.999, 100): # 100
            threshold = pointFunctionValue * ratio
            l0OfDistance, status = distanceL1FromCircle(threshold, point, solver, solverOptions)
            print('{0};{1};{2};{3}'.format(dimension, ratio, l0OfDistance, realDimensionReduction(point, threshold)))



