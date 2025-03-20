# Tournament-Sort

This is just a little experiment in sorting using consecutive tournament brackets. It's very rough and isn't meant to be used for anything other than satisfying curiosity.

I haven't analyzed its complexity (and probably never will), but a randomized sample input of 151 yields between 2500 and 2800 comparisons, depending on the randomization seed. Also, the result is broadly correct but not perfect.

The main heuristic is each "competitor's" win/loss ratio. Subsequent tournaments only include competitors that are undecided, defined as having a W:L that is zero or tied with another competitor.
