# Tournament-Sort

This is just a little experiment in sorting using consecutive tournaments. It's
very rough and isn't meant to be used for anything other than satisfying curiosity.

I haven't analyzed its complexity (and probably never will), but a randomized
sample input of 151 yields between 2500 and 2800 comparisons, depending on the
randomization seed. Also, the result is broadly correct but not perfect.

The heuristic used in generating brackets is each "competitor's" win/loss ratio.
Subsequent tournaments only include competitors that are undecided, defined as
having a W:L that is zero or tied with another competitor.

My idea at the start was to have an interface for applying a user's own comparison
criteria, such as something subjective like picking favorites. Most testing,
however, was done with matches won automatically based on alphabetic comparison.

![image](https://github.com/user-attachments/assets/17d53912-58b4-47ac-98c9-a1af44abcc81)
