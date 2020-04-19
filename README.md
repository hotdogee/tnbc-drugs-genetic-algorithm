# tnbc-drugs-genetic-algorithm

Generate novel drug combinations using Genetic Algorithm (GA) for Triple-negative breast cancer (TNBC)

# Languages and Libraries

* C#
* Windows Presentation Foundation (WPF)
* [WPFToolkit.DataVisualization](https://www.nuget.org/packages/WPFToolkit.DataVisualization/)

# Datasets

* [Connectivity Map (CMAP)](https://www.broadinstitute.org/connectivity-map-cmap)
* [DrugBank](https://www.drugbank.ca/)
* [KEGG DRUG](https://www.genome.jp/kegg/drug/)
* PPI
* TNBC Expressions
* NCBI Gene Symbol mapping

Put the `data` directory in the same location as `GA_Drugs.exe`
* [Download `data` directory from Google Drive](https://drive.google.com/drive/folders/1_v_jr53A258Yx5fs9ShBKat2QHfuDBbZ?usp=sharing)

# Features

* Real-time chart feedback
* Task manager
* Drug Browser
* Genetic Algorithm framework with configurable
  * Population Size (default: 100)
  * Crossover Method
  * Crossover Rate (default: 0.8)
  * Mutation Method
  * Mutation Rate (default: 0.5)
  * Selection Method
  * Elite Count (default: 5)
  * Max Generations (default: 500)
  * Stagnation Limit (default: 20)
* Crossover Methods
  * Single Point
  * Two Point - Middle
  * Two Point - Shortest
  * Uniform
  * Arithmetic
  * Heuristic
* Mutation Method
  * Single Point Roll
  * Single Bit Flip
  * Two Bit Swap
  * Inverse
* Selection Method
  * Roulette Wheel
  * Rank Selection
  * Tournament
  * Top Percent
  * Best
  * Random
  * Steady State Selection
  * Elitism
  * Family Competition

# Screenshots

![Main UI](screenshots/ga-drugs-2.png?raw=true "Main UI")
![Drug Browser](screenshots/ga-drugs-info.png?raw=true "Drug Browser")
