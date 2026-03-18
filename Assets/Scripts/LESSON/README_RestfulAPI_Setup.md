# Restful API UI Setup Guide

This guide will help you set up the complete UI system for the Restful API demo using restful-api.dev.

## Scripts Created

1. **RestfulAPIManager.cs** - Main controller script
2. **APIObjectItem.cs** - Individual item display script

## UI Setup Instructions

### 1. Create the Main Canvas

1. Right-click in Hierarchy → UI → Canvas
2. Add a **CanvasScaler** component and set to "Scale With Screen Size"
3. Set Reference Resolution to 1920x1080

### 2. Create the Main UI Structure

```
Canvas
├── Header Panel
│   ├── Title Text: "Restful API Demo"
│   ├── Refresh Button
│   └── Add Button (+)
├── Content Area
│   └── Scroll View
│       └── Content (Vertical Layout Group)
├── Add Panel (Initially disabled)
│   ├── Background Panel
│   ├── Name Input Field
│   ├── Data Input Field (Placeholder: "color:red, capacity:128GB")
│   ├── Send Button
│   └── Cancel Button
├── Edit Panel (Initially disabled)
│   ├── Background Panel
│   ├── Edit Name Input Field
│   ├── Edit Data Input Field
│   ├── Save Button
│   └── Cancel Edit Button
└── Status Text (Bottom)
```

### 3. Create the Item Prefab

1. Create an empty GameObject named "APIObjectItem"
2. Add the following structure:

```
APIObjectItem
├── Background Image (Panel)
├── Content Area (Horizontal Layout Group)
│   ├── Text Area (Vertical Layout Group)
│   │   ├── Name Text (Bold, larger font)
│   │   └── Data Text (Regular, smaller font)
│   └── Button Area (Horizontal Layout Group)
│       ├── Edit Button
│       └── Delete Button
```

### 4. Component Setup

#### RestfulAPIManager Script Assignment:
- **Content Parent**: Assign the Content object from the Scroll View
- **Item Prefab**: Assign the APIObjectItem prefab
- **Refresh Button**: Assign the refresh button
- **Add Button**: Assign the + button
- **Add Panel**: Assign the add panel GameObject
- **Name Input Field**: Assign the name input field
- **Data Input Field**: Assign the data input field
- **Send Button**: Assign the send button
- **Cancel Button**: Assign the cancel button
- **Edit Panel**: Assign the edit panel GameObject
- **Edit Name Input Field**: Assign the edit name input field
- **Edit Data Input Field**: Assign the edit data input field
- **Save Edit Button**: Assign the save button
- **Cancel Edit Button**: Assign the cancel edit button
- **Status Text**: Assign the status text at the bottom

#### APIObjectItem Script Assignment:
- **Name Text**: Assign the name text component
- **Data Text**: Assign the data text component
- **Edit Button**: Assign the edit button
- **Delete Button**: Assign the delete button

### 5. Layout Configuration

#### Scroll View Setup:
- Content Size Fitter: Vertical Fit = Preferred Size
- Vertical Layout Group: 
  - Child Controls Size: Width ✓, Height ✓
  - Child Force Expand: Width ✓, Height ✗
  - Spacing: 10

#### Item Prefab Layout:
- Content Size Fitter: Vertical Fit = Preferred Size
- Horizontal Layout Group for main content
- Vertical Layout Group for text area
- Horizontal Layout Group for buttons

### 6. Styling Recommendations

#### Colors:
- Background: Dark gray (#2D2D30)
- Item Background: Light gray (#3E3E42)
- Primary Button: Blue (#007ACC)
- Delete Button: Red (#E74C3C)
- Edit Button: Orange (#F39C12)

#### Text:
- Name Text: White, Bold, Size 18
- Data Text: Light Gray, Regular, Size 14
- Status Text: Yellow, Regular, Size 16

### 7. Input Field Configuration

#### Data Input Field:
- Placeholder Text: "color:red, capacity:128GB, generation:3rd, price:299"
- Character Limit: 200
- Content Type: Standard

## Usage

1. Press Play
2. The system will automatically load all objects from the API
3. Use the **Refresh** button to reload data
4. Use the **+** button to add new items
5. Each item has **Edit** and **Delete** buttons
6. Status updates appear at the bottom

## Data Format

When adding/editing items, use this format for the data field:
```
color:red, capacity:128GB, generation:3rd, price:299
```

The system supports these fields:
- color
- capacity  
- generation
- price

## API Endpoints Used

- **GET** `/objects` - Load all objects
- **POST** `/objects` - Create new object
- **PUT** `/objects/{id}` - Update existing object
- **DELETE** `/objects/{id}` - Delete object

## Troubleshooting

1. **Objects not loading**: Check internet connection and API status
2. **UI not responding**: Ensure all script references are assigned
3. **Data format errors**: Follow the key:value, key:value format exactly
4. **Buttons not working**: Check that Event System exists in scene