# Role Permissions Setup Guide

## Understanding Permission Flags

Permission flags use a bitwise system where each permission is a power of 2. They can be combined using the OR operator (|) to create composite permissions.

## Common Role Templates

### 1. System Administrator
```json
{
  "name": "System Administrator",
  "description": "Full system access with all permissions",
  "permissions": 8191  // All flags combined
}
```
**Includes:**
- All User Management (View, Edit, Delete)
- All Venue Management
- All Question Bank Management
- All Test Management
- View Soft Deleted Data

### 2. User Manager
```json
{
  "name": "User Manager",
  "description": "Manages user accounts and permissions",
  "permissions": 7  // ViewUsers | EditUsers | DeleteUsers
}
```
**Includes:**
- View Users (1)
- Edit Users (2)
- Delete Users (4)

### 3. Content Manager
```json
{
  "name": "Content Manager",
  "description": "Manages question banks and tests",
  "permissions": 3584  // ManageQuestionBanks | ManageTests
}
```
**Includes:**
- View Question Banks (64)
- Edit Question Banks (128)
- Delete Question Banks (256)
- View Tests (512)
- Edit Tests (1024)
- Delete Tests (2048)

### 4. Venue Manager
```json
{
  "name": "Venue Manager",
  "description": "Manages venue information",
  "permissions": 56  // ViewVenues | EditVenues | DeleteVenues
}
```
**Includes:**
- View Venues (8)
- Edit Venues (16)
- Delete Venues (32)

### 5. Read-Only User
```json
{
  "name": "Read Only",
  "description": "Can view all resources but cannot modify",
  "permissions": 585  // ViewUsers | ViewVenues | ViewQuestionBanks | ViewTests
}
```
**Includes:**
- View Users (1)
- View Venues (8)
- View Question Banks (64)
- View Tests (512)

## Permission Flag Reference

### User Permissions
- ViewUsers = 1
- EditUsers = 2
- DeleteUsers = 4
- ManageUsers = 7 (View | Edit | Delete)

### Venue Permissions
- ViewVenues = 8
- EditVenues = 16
- DeleteVenues = 32
- ManageVenues = 56 (View | Edit | Delete)

### Question Bank Permissions
- ViewQuestionBanks = 64
- EditQuestionBanks = 128
- DeleteQuestionBanks = 256
- ManageQuestionBanks = 448 (View | Edit | Delete)

### Test Permissions
- ViewTests = 512
- EditTests = 1024
- DeleteTests = 2048
- ManageTests = 3584 (View | Edit | Delete)

### Special Permissions
- ViewSoftDeletedData = 4096

## Creating Custom Role Combinations

To create custom combinations, add the numerical values of the desired permissions. For example:

### Test Creator with User View
```json
{
  "name": "Test Creator",
  "description": "Can create tests and view users",
  "permissions": 1537  // ViewUsers (1) | ViewTests (512) | EditTests (1024)
}
```

### Content Reviewer
```json
{
  "name": "Content Reviewer",
  "description": "Can view all content and soft-deleted data",
  "permissions": 4681  // ViewUsers (1) | ViewVenues (8) | ViewQuestionBanks (64) | ViewTests (512) | ViewSoftDeletedData (4096)
}
```

## Best Practices

1. **Principle of Least Privilege**
   - Start with minimal permissions and add as needed
   - Avoid granting delete permissions unless necessary

2. **Role Organization**
   - Keep roles focused on specific job functions
   - Document the purpose of each custom role
   - Regularly review and audit role permissions

3. **Permission Grouping**
   - Use the predefined management groups (ManageUsers, ManageVenues, etc.) when full access is needed
   - For limited access, combine specific permissions instead

4. **Testing New Roles**
   - Always test new role combinations in a non-production environment
   - Verify that all granted permissions work as expected
   - Check that denied actions are properly restricted

## API Request Examples

### Create Basic User Role
```http
POST /roles
Content-Type: application/json

{
  "name": "Basic User",
  "description": "Standard user with basic viewing permissions",
  "permissions": 585
}
```

### Create Custom Manager Role
```http
POST /roles
Content-Type: application/json

{
  "name": "Department Manager",
  "description": "Manages users and venues, can view all content",
  "permissions": 647  // ManageUsers | ManageVenues | ViewQuestionBanks | ViewTests
}
```
