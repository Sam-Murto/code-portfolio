#pragma once
#include <vector>
#include <iostream>

 #define GLM_ENABLE_EXPERIMENTAL
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <glm/gtx/quaternion.hpp>

class Curve
{
public:

	Curve();
	~Curve();
	
	void init();
	void calculate_curve();
	
public:
	float tau = 0.5; // Coefficient for catmull-rom spline
	int num_points_per_segment = 200;

	std::vector<glm::vec3> control_points_pos;
	std::vector<glm::vec3> curve_points_pos;
	std::vector<glm::quat> control_points_quaternion;
	bool enabled = true;

private:
	glm::mat3x4 get_control_mat(int index, int numControlPoints);
	glm::vec3 get_curve_point(float u, glm::mat3x4 control_mat);

	const glm::mat4 CATMULL = {
{-1.0f, 2.0f, -1.0f, 0.0f },
{3.0f, -5.0f, 0.0f, 2.0f },
{-3.0f, 4.0f, 1.0f, 0.0f },
{1.0f, -1.0f, 0.0f, 0.0f}
	};
};