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
        # CVXPY versions below 1.0 return numpy.matrx instead of array so convert
        solution = numpy.asarray(x.value).squeeze()
        # Make sure solution is in the feasible set or warn if not
        distance = solution.dot(solution) - threshold 
        if distance > 1e-4:
            print('*** WARNING for solver', solver, ': solution is', abs(distance), 'above threshold')
        # Return 
        return prob.value, 'optimal'
    except cvx.SolverError:
        return '-', 'failed'
    

def treatLine(line):
    tokens = line.replace('\n', '').replace('\r', '').split(' ')
    threshold = float(tokens[1])
    point = list(map(float, tokens[2:]))
    sys.stdout.write('started computing\n')
    result = distanceL1FromCircle(threshold, point)
    sys.stdout.write(str(result) + '\n')
    sys.stdout.flush()

if __name__=='__main__':
    # Solvers and parameters
    solver_options = [ ('SCS', dict(eps=1e-5, max_iters=100000)), 
                       ('ECOS',  dict(max_iters=1000)),
                       ('CVXOPT',  dict(kktsolver=cvx.ROBUST_KKTSOLVER, max_iters=1000))
                     ]
    for i in range(30):
        value = 2 ** i
        dimesion = 10
        point = [value for _ in range(dimesion)]
        diff = 1.0
        threshold = sum([v * v for v in point]) - diff
        print('iteration', i, 'threshold: ', threshold)
        for solver, options in solver_options:
            objective, status = distanceL1FromCircle(threshold, point, solver, options)
            print('    {}: {} ({})'.format(solver, objective, status))
        print()
        


