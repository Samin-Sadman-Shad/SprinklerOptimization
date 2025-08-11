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

<img width="2048" height="1024" alt="Raycasting_2" src="https://github.com/user-attachments/assets/279bed78-809a-46a5-85c7-bb5667f35305" />



**Professional Architecture**

Dependency injection pattern<br/>
Interface-driven design<br/>
Open-Closed principle with Factory Pattern<br/>
Comprehensive unit testing<br/>
Quality metrics calculation<br/>


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

  Calculates room bounding box,<br/>
  Places sprinklers at regular intervals in an uniform grid pattern (2500mm spacing),<br/>
  Maintains wall clearances<br/>

<img width="571" height="390" alt="grid_layout2" src="https://github.com/user-attachments/assets/6676dae6-6089-442e-8b22-9075fa3ac909" />
<img width="800" height="600" alt="SprinklerLayout2DGrid" src="https://github.com/user-attachments/assets/e838568c-2f4b-43a9-b176-ee7ef7bb8b4e" />



2. Optimized Coverage Strategy
Uses geometric analysis for maximum coverage:<br/>

Start<br/>
  ↓<br/>
compute centroid -> if valid add to sprinklers<br/>
  ↓<br/>
while iteration < maxIterations:<br/>
  FindNextOptimalPosition:<br/>
    - grid scan over (x,y)<br/>
    - skip if outside or too near walls<br/>
    - skip if too close to other sprinklers<br/>
    - score candidate by sampling uncovered points<br/>
  if best candidate is null -> break<br/>
  else add sprinkler and continue<br/>
  ↓<br/>
return sprinklers<br/>

<img width="571" height="390" alt="coverage_optimization5" src="https://github.com/user-attachments/assets/935d1c75-4a77-4730-a73b-0c35c55bedb7" />
<img width="800" height="600" alt="SprinklerLayout2DMaximumCoverage" src="https://github.com/user-attachments/assets/72138481-dc39-4365-a4e0-4994c8394be4" />



3. Pipe Proximity Strategy
Minimizes connection distances to existing pipes:<br/>

  -sampling points along each pipe (projected to the ceiling),<br/>
  -filtering those points to stay inside the room and respect wall clearance,<br/>
  -sorting the candidate points by how close they are to pipes (closest first),<br/>
  -greedily adding candidates while enforcing minimum spacing between sprinklers.<br/>

<img width="571" height="391" alt="pipe_proximity" src="https://github.com/user-attachments/assets/b30c5b16-e115-442c-bf32-6584e4295576" />
<img width="800" height="600" alt="SprinklerLayout2DMinimumPipeDistance" src="https://github.com/user-attachments/assets/7444cf12-656c-4463-943e-71d890d9ba38" />



