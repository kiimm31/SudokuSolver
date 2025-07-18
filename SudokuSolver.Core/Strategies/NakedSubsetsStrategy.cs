using SudokuSolver.Core.Base;
using SudokuSolver.Core.Helpers;
using SudokuSolver.Domain.Models;

namespace SudokuSolver.Core.Strategies;

/// <summary>
/// Strategy that finds naked subsets (pairs, triples, quads) and eliminates their candidates from other cells
/// </summary>
public class NakedSubsetsStrategy : BaseSolvingStrategy
{
    public override string Name => "Naked Subsets Strategy";
    public override int Priority => 2;

    protected override void ApplyToGroup(List<Cell> targetGroup)
    {
        var unsolvedCells = targetGroup.Where(c => !c.IsSolved).ToList();
        
        if (unsolvedCells.Count < 2)
            return;

        // Try to find naked pairs, triples, and quads
        FindNakedPairs(unsolvedCells);
        FindNakedTriples(unsolvedCells);
        FindNakedQuads(unsolvedCells);
    }

    private static void FindNakedPairs(List<Cell> cells)
    {
        // Find cells with exactly 2 candidates
        var pairCandidates = cells.Where(c => c.GetPossibleValues().Count == 2).ToList();
        
        for (int i = 0; i < pairCandidates.Count; i++)
        {
            for (int j = i + 1; j < pairCandidates.Count; j++)
            {
                var cell1 = pairCandidates[i];
                var cell2 = pairCandidates[j];
                
                // Check if they have the same candidates (naked pair)
                if (cell1.IsPair(cell2))
                {
                    var pairValues = cell1.GetPossibleValues();
                    EliminateFromOtherCells(cells, [cell1, cell2], pairValues);
                }
            }
        }
    }

    private static void FindNakedTriples(List<Cell> cells)
    {
        // Find cells with 2 or 3 candidates that could form triples
        var tripleCandidates = cells.Where(c => c.GetPossibleValues().Count >= 2 && c.GetPossibleValues().Count <= 3).ToList();
        
        for (int i = 0; i < tripleCandidates.Count; i++)
        {
            for (int j = i + 1; j < tripleCandidates.Count; j++)
            {
                for (int k = j + 1; k < tripleCandidates.Count; k++)
                {
                    var cell1 = tripleCandidates[i];
                    var cell2 = tripleCandidates[j];
                    var cell3 = tripleCandidates[k];
                    
                    var allCandidates = cell1.GetPossibleValues()
                        .Union(cell2.GetPossibleValues())
                        .Union(cell3.GetPossibleValues())
                        .ToList();
                    
                    // If these three cells contain exactly 3 candidates total, it's a naked triple
                    if (allCandidates.Count == 3)
                    {
                        // Each cell must be a subset of these 3 candidates
                        if (IsSubset(cell1.GetPossibleValues(), allCandidates) &&
                            IsSubset(cell2.GetPossibleValues(), allCandidates) &&
                            IsSubset(cell3.GetPossibleValues(), allCandidates))
                        {
                            EliminateFromOtherCells(cells, [cell1, cell2, cell3], allCandidates);
                        }
                    }
                }
            }
        }
    }

    private static void FindNakedQuads(List<Cell> cells)
    {
        // Find cells with 2, 3, or 4 candidates that could form quads
        var quadCandidates = cells.Where(c => c.GetPossibleValues().Count >= 2 && c.GetPossibleValues().Count <= 4).ToList();
        
        for (int i = 0; i < quadCandidates.Count; i++)
        {
            for (int j = i + 1; j < quadCandidates.Count; j++)
            {
                for (int k = j + 1; k < quadCandidates.Count; k++)
                {
                    for (int l = k + 1; l < quadCandidates.Count; l++)
                    {
                        var cell1 = quadCandidates[i];
                        var cell2 = quadCandidates[j];
                        var cell3 = quadCandidates[k];
                        var cell4 = quadCandidates[l];
                        
                        var allCandidates = cell1.GetPossibleValues()
                            .Union(cell2.GetPossibleValues())
                            .Union(cell3.GetPossibleValues())
                            .Union(cell4.GetPossibleValues())
                            .ToList();
                        
                        // If these four cells contain exactly 4 candidates total, it's a naked quad
                        if (allCandidates.Count == 4)
                        {
                            // Each cell must be a subset of these 4 candidates
                            if (IsSubset(cell1.GetPossibleValues(), allCandidates) &&
                                IsSubset(cell2.GetPossibleValues(), allCandidates) &&
                                IsSubset(cell3.GetPossibleValues(), allCandidates) &&
                                IsSubset(cell4.GetPossibleValues(), allCandidates))
                            {
                                EliminateFromOtherCells(cells, [cell1, cell2, cell3, cell4], allCandidates);
                            }
                        }
                    }
                }
            }
        }
    }

    private static void EliminateFromOtherCells(List<Cell> allCells, List<Cell> subsetCells, List<int> candidatesToEliminate)
    {
        var otherCells = allCells.Where(c => !subsetCells.Contains(c)).ToList();
        
        foreach (var cell in otherCells)
        {
            foreach (var candidate in candidatesToEliminate)
            {
                cell.EliminatePossibleValue(candidate);
            }
        }
    }

    private static bool IsSubset(List<int> subset, List<int> superset)
    {
        return subset.All(superset.Contains);
    }
}