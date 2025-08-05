import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import os

# Path to the CSV file
csv_path = '/workspaces/BlazorApp/TodoApi/TodoApi.Tests/Performance/performance_results.csv'

if not os.path.exists(csv_path):
    print(f'CSV file not found: {csv_path}')
    exit(1)

# Read the CSV file
df = pd.read_csv(csv_path, names=['Scenario', 'ResponseTimeMs', 'Timestamp'])

# Summary statistics
print('Summary statistics:')
print(df.groupby('Scenario')['ResponseTimeMs'].describe())

# Plot response times by scenario
plt.figure(figsize=(10, 6))
sns.boxplot(x='Scenario', y='ResponseTimeMs', data=df)
plt.title('API Response Time by Scenario')
plt.ylabel('Response Time (ms)')
plt.xlabel('Scenario')
plt.tight_layout()
plt.savefig('/workspaces/BlazorApp/TodoApi/TodoApi.Tests/Performance/performance_boxplot.png')
plt.show()

# Plot time series for each scenario
plt.figure(figsize=(12, 6))
for scenario in df['Scenario'].unique():
    subset = df[df['Scenario'] == scenario]
    plt.plot(pd.to_datetime(subset['Timestamp']), subset['ResponseTimeMs'], label=scenario)
plt.title('API Response Time Over Time')
plt.ylabel('Response Time (ms)')
plt.xlabel('Timestamp')
plt.legend()
plt.tight_layout()
plt.savefig('/workspaces/BlazorApp/TodoApi/TodoApi.Tests/Performance/performance_timeseries.png')
plt.show()

print('Charts saved as performance_boxplot.png and performance_timeseries.png in the Performance folder.')
