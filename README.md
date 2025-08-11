A C# application that calculates optimal fire sprinkler placements for irregular room geometries using advanced spatial reasoning and multiple optimization strategies.
🏗️ **System Overview**
Automatically generates fire sprinkler layouts considering water pipe locations, wall clearances, and coverage requirements using professional software engineering practices.
✨ Key Features

**Three Core Placement Strategies**

Uniform Grid: Systematic grid placement ensuring code compliance
Optimized Coverage: Geometry-based placement maximizing room coverage
Pipe Proximity: Cost-optimized placement minimizing connection distances


**Advanced Spatial Reasoning**

3D geometric calculations and vector operations
Point-in-polygon testing with ray casting algorithm
Optimal pipe connection point determination


**Professional Architecture**

Dependency injection pattern
Interface-driven design
Open-Closed principle with Factory Pattern
Comprehensive unit testing
Quality metrics calculation


🚀 **Quick Start**
Prerequisites

.NET 8.0 or later
Microsoft.Extensions.DependencyInjection
Microsoft.Extensions.Logging

Running the Application
bash# Clone and build
git clone <repository-url>
cd SprinklerLayoutSystem
dotnet build

Run the application
dotnet run

🔧 **Core Strategies**
1. Uniform Grid Strategy
Creates systematic grid-based sprinkler placement:

  Calculates room bounding box,
  Places sprinklers at regular intervals in an uniform grid pattern (2500mm spacing),
  Maintains wall clearances,

2. Optimized Coverage Strategy
Uses geometric analysis for maximum coverage:

Start
  ↓
compute centroid -> if valid add to sprinklers
  ↓
while iteration < maxIterations:
  FindNextOptimalPosition:
    - grid scan over (x,y)
    - skip if outside or too near walls
    - skip if too close to other sprinklers
    - score candidate by sampling uncovered points
  if best candidate is null -> break
  else add sprinkler and continue
  ↓
return sprinklers

3. Pipe Proximity Strategy
Minimizes connection distances to existing pipes:

  -sampling points along each pipe (projected to the ceiling),
  -filtering those points to stay inside the room and respect wall clearance,
  -sorting the candidate points by how close they are to pipes (closest first),
  -greedily adding candidates while enforcing minimum spacing between sprinklers.

