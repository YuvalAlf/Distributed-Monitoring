namespace EntropyMathematics

type FastCountSet<'T when 'T : comparison>(set : 'T Set, count : int) =
    member this.Count = count
    member this.ItemsSet = set
    static member Union (set1 : FastCountSet<'T>) (set2 : FastCountSet<'T>) =
        FastCountSet(Set.union set1.ItemsSet set2.ItemsSet, set1.Count + set2.Count)
    static member Create (set : Set<'T>) = FastCountSet(set, set.Count)