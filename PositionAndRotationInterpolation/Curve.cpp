#include "Curve.h"

Curve::Curve()
{
}

Curve::~Curve()
{
}

void Curve::init()
{
	this->control_points_pos = 
	{
		{ 0.0, 8.5, -2.0 },
		{ -3.0, 11.0, 2.3 },
		{ -6.0, 8.5, -2.5 },
		{ -4.0, 5.5, 2.8 },
		{ 1.0, 2.0, -4.0 },
		{ 4.0, 2.0, 3.0 },
		{ 7.0, 8.0, -2.0 },
		{ 3.0, 10.0, 3.7 }
	};
	calculate_curve();
	
	this->control_points_quaternion = {
		{0.13964   , 0.0481732 , 0.831429 , 0.541043 , },
		{0.0509038 , -0.033869 , -0.579695, 0.811295 , },
		{-0.502889 , -0.366766 , 0.493961 , 0.592445 , },
		{-0.636    , 0.667177  , -0.175206, 0.198922 , },
		{0.693492  , 0.688833  , -0.152595, -0.108237, },
		{0.752155  , -0.519591 , -0.316988, 0.168866 , },
		{0.542054  , 0.382705  , 0.378416 , 0.646269 , },
		{0.00417342, -0.0208652, -0.584026, 0.810619   }
	};
}

void Curve::calculate_curve()
{
	curve_points_pos.clear();

	int numControlPoints = control_points_pos.size();
	float u_increment = 1.0f / num_points_per_segment;

	//Go through each control point
	for (int i = 0; i < numControlPoints; i++)
	{

		//Tranpose for use in catmull-rom
		glm::mat3x4 control_mat = get_control_mat(i, numControlPoints);

		// Push curve points into curve point vector. Floating point error results in 1 extra iteration
		for (float u = 0.0f; u < 1.0f - u_increment; u += u_increment)
		{
			curve_points_pos.push_back(get_curve_point(u, control_mat));
		}
	}
}

// Will return a matrix with the necessary points for control_point_pos[index]'s catmull-rom curve
glm::mat3x4 Curve::get_control_mat(int index, int numControlPoints)
{
	//Put control points in a 4x3 matrix
	glm::mat4x3 controls;
	for (int j = index - 1, count = 0; count < 4; j++, count++) {
		if (j < 0)
			j = numControlPoints - 1;

		if (j > numControlPoints - 1)
			j = 0;

		controls[count] = control_points_pos[j];

	}

	return glm::transpose(controls);

}

// Interpolation function for catmull-rom curve
glm::vec3 Curve::get_curve_point(float u, glm::mat3x4 control_mat)
{
	float uu = u * u;
	float uuu = uu * u;

	return this->tau * glm::vec4(uuu, uu, u, 1) * CATMULL * control_mat;
}